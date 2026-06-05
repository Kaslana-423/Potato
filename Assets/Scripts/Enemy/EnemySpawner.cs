using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum EnemySpawnBatchMode
{
    Single,
    Group,
    Mixed
}

[Serializable]
public sealed class EnemyPrefabBinding
{
    [Tooltip("Use an id from EnemyCatalog, for example enemy.baby_alien or enemy.chaser.")]
    public string enemyId = "enemy.baby_alien";

    public EnemyBase prefab;
}

[Serializable]
public sealed class EnemyWaveEnemySpawnRule
{
    [Tooltip("Use an id from EnemyCatalog, for example enemy.baby_alien or enemy.chaser.")]
    public string enemyId = "enemy.baby_alien";

    [Header("Frequency")]
    [Tooltip("Base seconds between this enemy's spawn batches.")]
    [Min(0.05f)] public float baseSpawnInterval = 1f;

    [Tooltip("X = wave offset from this wave config start, Y = interval multiplier.")]
    public AnimationCurve intervalMultiplierByWaveOffset = AnimationCurve.Linear(0f, 1f, 10f, 0.75f);

    [Tooltip("X = progress inside the current wave from 0 to 1, Y = interval multiplier.")]
    public AnimationCurve intervalMultiplierOverWaveProgress = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.5f);

    [Tooltip("Fastest allowed interval for this enemy.")]
    [Min(0.05f)] public float minimumSpawnInterval = 0.2f;

    [Header("Batch")]
    public EnemySpawnBatchMode batchMode = EnemySpawnBatchMode.Single;

    [Tooltip("Used by Single and Mixed modes.")]
    [Min(1)] public int singleBatchMin = 1;

    [Tooltip("Used by Single and Mixed modes.")]
    [Min(1)] public int singleBatchMax = 1;

    [Tooltip("Used by Group and Mixed modes.")]
    [Min(1)] public int groupBatchMin = 3;

    [Tooltip("Used by Group and Mixed modes.")]
    [Min(1)] public int groupBatchMax = 6;

    [Tooltip("Used only by Mixed mode.")]
    [Range(0f, 1f)] public float groupChance = 0.35f;

    [Tooltip("How far members of the same group spread around the chosen spawn point.")]
    [Min(0f)] public float groupSpreadRadius = 1.2f;
}

[Serializable]
public sealed class EnemyWaveSpawnSettings
{
    [Tooltip("This config is used from this wave until a later config takes over.")]
    [Min(1)] public int startWave = 1;

    [Header("Duration")]
    [Tooltip("Use the duration from enemies.xlsx Waves sheet through EnemyCatalog.")]
    public bool useCatalogWaveDuration = true;

    [Tooltip("Used only when Use Catalog Wave Duration is false.")]
    [Min(1f)] public float waveDurationSeconds = 60f;

    [Header("Enemy Rules")]
    public List<EnemyWaveEnemySpawnRule> enemyRules = new List<EnemyWaveEnemySpawnRule>();
}

public sealed class EnemySpawner : MonoBehaviour
{
    [Header("Wave")]
    [SerializeField, Min(1)] private int currentWave = 1;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool clearAliveEnemiesWhenLevelEnds = true;

    [Header("Shop Flow")]
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private GameObject shopRoot;
    [SerializeField] private Button shopExitButton;
    [SerializeField, Min(0f)] private float shopOpenDelaySeconds = 2f;
    [SerializeField, Min(0f)] private float nextLevelDelaySeconds = 1f;
    [SerializeField] private bool hideShopOnStart = true;
    [SerializeField] private bool refreshShopWhenOpened = true;

    [Header("Wave Plan")]
    [SerializeField] private bool useConfiguredWaves = true;
    [SerializeField] private List<EnemyWaveSpawnSettings> waveSettings = new List<EnemyWaveSpawnSettings>();

    [Header("Fallback Wave Plan")]
    [SerializeField, Min(0.05f)] private float fallbackSpawnInterval = 1f;
    [SerializeField, Min(1)] private int fallbackEnemiesPerSpawn = 1;

    [Header("Enemy Pool")]
    [SerializeField] private bool includeDlcEnemies = false;
    [SerializeField] private bool includeElites = false;
    [SerializeField] private bool includeBosses = false;

    [Header("Spawn")]
    [SerializeField] private EnemyBase defaultEnemyPrefab;
    [SerializeField] private List<EnemyPrefabBinding> enemyPrefabs = new List<EnemyPrefabBinding>();
    [SerializeField] private Transform playerTarget;
    [SerializeField, Min(0f)] private float spawnRadius = 12f;

    [Header("Map Bounds")]
    [SerializeField] private bool restrictSpawnToMapBounds = true;
    [SerializeField] private Collider2D mapBoundsCollider;
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(32f, 18f);
    [SerializeField, Min(0f)] private float mapBoundsPadding = 0.5f;
    [SerializeField, Min(1)] private int spawnPositionAttempts = 12;

    [Header("Spawn Warning")]
    [SerializeField] private EnemySpawnWarning spawnWarningPrefab;
    [SerializeField, Min(0f)] private float spawnWarningSeconds = 0.65f;
    [SerializeField, Min(0.1f)] private float spawnWarningRadius = 0.75f;

    private readonly List<EnemyBase> aliveEnemies = new List<EnemyBase>();
    private Coroutine spawnRoutine;
    private Coroutine nextLevelRoutine;
    private Button boundShopExitButton;
    private bool levelRunning;
    private int levelRunId;
    private static Sprite fallbackSprite;

    public int CurrentWave => currentWave;
    public int AliveCount => aliveEnemies.Count(enemy => enemy != null);
    public bool IsLevelRunning => levelRunning;

    private void Reset()
    {
        EnsureDefaultWaveSettings();
    }

    private void OnValidate()
    {
        currentWave = Mathf.Max(1, currentWave);
        fallbackSpawnInterval = Mathf.Max(0.05f, fallbackSpawnInterval);
        fallbackEnemiesPerSpawn = Mathf.Max(1, fallbackEnemiesPerSpawn);
        spawnRadius = Mathf.Max(0f, spawnRadius);
        spawnAreaSize = new Vector2(Mathf.Max(0.1f, spawnAreaSize.x), Mathf.Max(0.1f, spawnAreaSize.y));
        mapBoundsPadding = Mathf.Max(0f, mapBoundsPadding);
        spawnPositionAttempts = Mathf.Max(1, spawnPositionAttempts);
        shopOpenDelaySeconds = Mathf.Max(0f, shopOpenDelaySeconds);
        nextLevelDelaySeconds = Mathf.Max(0f, nextLevelDelaySeconds);
        spawnWarningSeconds = Mathf.Max(0f, spawnWarningSeconds);
        spawnWarningRadius = Mathf.Max(0.1f, spawnWarningRadius);
        EnsureDefaultWaveSettings();
        ValidateWaveSettings();
    }

    private void Start()
    {
        AutoBindFlowReferences();
        BindShopExitButton();

        if (hideShopOnStart)
        {
            SetShopVisible(false);
        }

        if (playerTarget == null)
        {
            FindPlayerTarget();
        }

        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    private void OnDisable()
    {
        StopSpawning();
        UnbindShopExitButton();
    }

    [ContextMenu("Start Spawning")]
    public void StartSpawning()
    {
        StopSpawning();
        levelRunId++;
        levelRunning = true;
        spawnRoutine = StartCoroutine(SpawnWaveLoop());
    }

    [ContextMenu("Stop Spawning")]
    public void StopSpawning()
    {
        levelRunning = false;
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        if (nextLevelRoutine != null)
        {
            StopCoroutine(nextLevelRoutine);
            nextLevelRoutine = null;
        }
    }

    public void SetWave(int wave)
    {
        currentWave = Mathf.Max(1, wave);
    }

    public void StartWave(int wave)
    {
        SetWave(wave);
        SetShopVisible(false);
        StartSpawning();
    }

    public void ExitShopAndStartNextLevel()
    {
        if (nextLevelRoutine != null)
        {
            StopCoroutine(nextLevelRoutine);
        }

        nextLevelRoutine = StartCoroutine(ExitShopAndStartNextLevelRoutine());
    }

    public void SpawnOne()
    {
        EnemyDefinition definition = PickFallbackEnemyDefinition();
        if (definition != null)
        {
            SpawnEnemy(definition, GetSpawnPosition());
        }
    }

    private IEnumerator SpawnWaveLoop()
    {
        int runId = levelRunId;
        List<EnemySpawnRuntimeState> states = BuildSpawnStates(currentWave);
        if (states.Count == 0)
        {
            levelRunning = false;
            spawnRoutine = null;
            yield break;
        }

        float elapsed = 0f;
        float waveDuration = GetWaveDuration(currentWave);

        while (elapsed < waveDuration)
        {
            TrimDeadEnemies();

            EnemySpawnRuntimeState readyState = GetNextReadyState(states, elapsed);
            if (readyState == null)
            {
                float waitTime = Mathf.Min(GetTimeUntilNextSpawn(states, elapsed), waveDuration - elapsed);
                yield return new WaitForSeconds(waitTime);
                elapsed += waitTime;
                continue;
            }

            float progress = waveDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / waveDuration);
            ScheduleSpawnBatch(readyState, runId);
            readyState.nextSpawnTime = elapsed + GetSpawnInterval(readyState, progress);

            yield return null;
        }

        levelRunning = false;
        spawnRoutine = null;
        if (clearAliveEnemiesWhenLevelEnds)
        {
            DestroyAliveEnemies();
        }

        yield return new WaitForSeconds(shopOpenDelaySeconds);
        if (runId == levelRunId)
        {
            OpenShop();
        }
    }

    private List<EnemySpawnRuntimeState> BuildSpawnStates(int wave)
    {
        EnemyWaveSpawnSettings settings = GetWaveSettings(wave);
        if (settings == null || settings.enemyRules == null || settings.enemyRules.Count == 0)
        {
            return BuildFallbackSpawnStates(wave);
        }

        int waveOffset = Mathf.Max(0, wave - Mathf.Max(1, settings.startWave));
        var states = new List<EnemySpawnRuntimeState>();
        foreach (EnemyWaveEnemySpawnRule rule in settings.enemyRules)
        {
            if (rule == null || !EnemyCatalog.TryGetById(rule.enemyId, out EnemyDefinition enemy) || !CanSpawnEnemy(enemy, wave))
            {
                continue;
            }

            states.Add(new EnemySpawnRuntimeState(enemy, rule, waveOffset));
        }

        return states;
    }

    private List<EnemySpawnRuntimeState> BuildFallbackSpawnStates(int wave)
    {
        List<EnemyDefinition> eligibleEnemies = EnemyCatalog
            .GetEligibleEnemies(wave, includeDlcEnemies, includeElites, includeBosses)
            .ToList();

        if (eligibleEnemies.Count == 0)
        {
            return new List<EnemySpawnRuntimeState>();
        }

        var states = new List<EnemySpawnRuntimeState>();

        foreach (EnemyDefinition enemy in eligibleEnemies)
        {
            var rule = new EnemyWaveEnemySpawnRule
            {
                enemyId = enemy.Id,
                baseSpawnInterval = fallbackSpawnInterval,
                minimumSpawnInterval = fallbackSpawnInterval,
                batchMode = EnemySpawnBatchMode.Single,
                singleBatchMin = fallbackEnemiesPerSpawn,
                singleBatchMax = fallbackEnemiesPerSpawn
            };

            states.Add(new EnemySpawnRuntimeState(enemy, rule, Mathf.Max(0, wave - 1)));
        }

        return states;
    }

    private int ScheduleSpawnBatch(EnemySpawnRuntimeState state, int runId)
    {
        if (state == null)
        {
            return 0;
        }

        bool spawnGroup = ShouldSpawnGroup(state.rule);
        int batchSize = spawnGroup
            ? UnityEngine.Random.Range(state.rule.groupBatchMin, state.rule.groupBatchMax + 1)
            : UnityEngine.Random.Range(state.rule.singleBatchMin, state.rule.singleBatchMax + 1);

        Vector3 center = GetSpawnPosition();
        List<Vector3> positions = spawnGroup
            ? BuildGroupSpawnPositions(center, batchSize, state.rule.groupSpreadRadius)
            : BuildSingleSpawnPositions(center, batchSize);

        for (int index = 0; index < positions.Count; index++)
        {
            ShowSpawnWarning(positions[index]);
        }

        StartCoroutine(SpawnAfterWarning(state.definition, positions, runId));
        return batchSize;
    }

    private List<Vector3> BuildSingleSpawnPositions(Vector3 center, int batchSize)
    {
        var positions = new List<Vector3>(batchSize);
        Vector3 clampedCenter = ClampToMapBounds(center);
        for (int index = 0; index < batchSize; index++)
        {
            positions.Add(clampedCenter);
        }

        return positions;
    }

    private List<Vector3> BuildGroupSpawnPositions(Vector3 center, int batchSize, float spreadRadius)
    {
        var positions = new List<Vector3>(batchSize);
        if (batchSize <= 0)
        {
            return positions;
        }

        if (batchSize == 1 || spreadRadius <= 0f)
        {
            positions.Add(ClampToMapBounds(center));
            return positions;
        }

        float angleOffset = UnityEngine.Random.Range(0f, 360f);
        for (int index = 0; index < batchSize; index++)
        {
            float angle = angleOffset + (360f * index / batchSize);
            float radians = angle * Mathf.Deg2Rad;
            float ringRadius = spreadRadius * UnityEngine.Random.Range(0.65f, 1f);
            Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * ringRadius;
            positions.Add(ClampToMapBounds(center + offset));
        }

        return positions;
    }

    private bool ShouldSpawnGroup(EnemyWaveEnemySpawnRule rule)
    {
        switch (rule.batchMode)
        {
            case EnemySpawnBatchMode.Group:
                return true;
            case EnemySpawnBatchMode.Mixed:
                return UnityEngine.Random.value < rule.groupChance;
            default:
                return false;
        }
    }

    private IEnumerator SpawnAfterWarning(EnemyDefinition definition, IReadOnlyList<Vector3> positions, int runId)
    {
        if (spawnWarningSeconds > 0f)
        {
            yield return new WaitForSeconds(spawnWarningSeconds);
        }

        if (!levelRunning || runId != levelRunId)
        {
            yield break;
        }

        for (int index = 0; index < positions.Count; index++)
        {
            SpawnEnemy(definition, positions[index]);
        }
    }

    private void ShowSpawnWarning(Vector3 position)
    {
        EnemySpawnWarning warning = spawnWarningPrefab != null
            ? Instantiate(spawnWarningPrefab, position, Quaternion.identity)
            : CreateDefaultSpawnWarning(position);

        warning.Play(position, spawnWarningSeconds, spawnWarningRadius);
    }

    private EnemySpawnWarning CreateDefaultSpawnWarning(Vector3 position)
    {
        GameObject warningObject = new GameObject("Enemy Spawn Warning");
        warningObject.transform.position = position;
        return warningObject.AddComponent<EnemySpawnWarning>();
    }

    private float GetSpawnInterval(EnemySpawnRuntimeState state, float waveProgress)
    {
        float interval = state.rule.baseSpawnInterval;
        interval *= EvaluateMultiplier(state.rule.intervalMultiplierByWaveOffset, state.waveOffset);
        interval *= EvaluateMultiplier(state.rule.intervalMultiplierOverWaveProgress, waveProgress);
        return Mathf.Max(state.rule.minimumSpawnInterval, interval);
    }

    private EnemySpawnRuntimeState GetNextReadyState(IReadOnlyList<EnemySpawnRuntimeState> states, float elapsed)
    {
        EnemySpawnRuntimeState selected = null;
        foreach (EnemySpawnRuntimeState state in states)
        {
            if (state.nextSpawnTime > elapsed)
            {
                continue;
            }

            if (selected == null || state.nextSpawnTime < selected.nextSpawnTime)
            {
                selected = state;
            }
        }

        return selected;
    }

    private float GetTimeUntilNextSpawn(IReadOnlyList<EnemySpawnRuntimeState> states, float elapsed)
    {
        float nextTime = float.PositiveInfinity;
        foreach (EnemySpawnRuntimeState state in states)
        {
            if (state.nextSpawnTime < nextTime)
            {
                nextTime = state.nextSpawnTime;
            }
        }

        if (float.IsPositiveInfinity(nextTime))
        {
            return 0.1f;
        }

        return Mathf.Clamp(nextTime - elapsed, 0.01f, 0.5f);
    }

    private EnemyWaveSpawnSettings GetWaveSettings(int wave)
    {
        if (!useConfiguredWaves || waveSettings == null || waveSettings.Count == 0)
        {
            return null;
        }

        EnemyWaveSpawnSettings selected = null;
        int clampedWave = Mathf.Max(1, wave);
        foreach (EnemyWaveSpawnSettings settings in waveSettings)
        {
            if (settings == null || settings.startWave > clampedWave)
            {
                continue;
            }

            if (selected == null || settings.startWave > selected.startWave)
            {
                selected = settings;
            }
        }

        return selected;
    }

    private float GetWaveDuration(int wave)
    {
        EnemyWaveSpawnSettings settings = GetWaveSettings(wave);
        if (settings != null && !settings.useCatalogWaveDuration)
        {
            return Mathf.Max(1f, settings.waveDurationSeconds);
        }

        return EnemyCatalog.GetWaveDurationSeconds(wave);
    }

    private bool CanSpawnEnemy(EnemyDefinition enemy, int wave)
    {
        return enemy != null
            && enemy.FirstWave <= wave
            && (includeDlcEnemies || !enemy.IsDlc)
            && (includeElites || !enemy.IsElite)
            && (includeBosses || !enemy.IsBoss);
    }

    private EnemyDefinition PickFallbackEnemyDefinition()
    {
        List<EnemyDefinition> eligibleEnemies = EnemyCatalog
            .GetEligibleEnemies(currentWave, includeDlcEnemies, includeElites, includeBosses)
            .ToList();

        if (eligibleEnemies.Count == 0)
        {
            return null;
        }

        return eligibleEnemies[UnityEngine.Random.Range(0, eligibleEnemies.Count)];
    }

    private IEnumerator ExitShopAndStartNextLevelRoutine()
    {
        SetShopVisible(false);
        if (nextLevelDelaySeconds > 0f)
        {
            yield return new WaitForSeconds(nextLevelDelaySeconds);
        }

        currentWave++;
        nextLevelRoutine = null;
        StartSpawning();
    }

    private void OpenShop()
    {
        AutoBindFlowReferences();
        SetShopVisible(true);

        if (refreshShopWhenOpened && shopManager != null)
        {
            shopManager.RefreshShop();
        }

        BindShopExitButton();
    }

    private void SetShopVisible(bool visible)
    {
        if (shopRoot != null && shopRoot.activeSelf != visible)
        {
            shopRoot.SetActive(visible);
        }

        if (shopManager != null)
        {
            shopManager.SetShopOpen(visible);
            return;
        }
    }

    private void AutoBindFlowReferences()
    {
        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>();
        }

        if (shopRoot == null && shopManager != null)
        {
            Canvas canvas = shopManager.GetComponentInChildren<Canvas>(true);
            if (canvas != null && canvas.gameObject != shopManager.gameObject)
            {
                shopRoot = canvas.gameObject;
            }
        }

        if (shopExitButton == null && shopRoot != null)
        {
            shopExitButton = FindButtonInChildren(
                shopRoot.transform,
                "NextWaveButton",
                "Next Wave Button",
                "StartNextWaveButton",
                "Start Next Wave Button",
                "ExitShopButton",
                "Exit Shop Button");
        }

        if (shopExitButton == null && shopManager != null)
        {
            shopExitButton = FindButtonInChildren(
                shopManager.transform,
                "NextWaveButton",
                "Next Wave Button",
                "StartNextWaveButton",
                "Start Next Wave Button",
                "ExitShopButton",
                "Exit Shop Button");
        }
    }

    private void BindShopExitButton()
    {
        if (boundShopExitButton == shopExitButton)
        {
            return;
        }

        UnbindShopExitButton();
        boundShopExitButton = shopExitButton;
        if (boundShopExitButton != null)
        {
            boundShopExitButton.onClick.AddListener(ExitShopAndStartNextLevel);
        }
    }

    private void UnbindShopExitButton()
    {
        if (boundShopExitButton != null)
        {
            boundShopExitButton.onClick.RemoveListener(ExitShopAndStartNextLevel);
            boundShopExitButton = null;
        }
    }

    private static Button FindButtonInChildren(Transform root, params string[] names)
    {
        if (root == null)
        {
            return null;
        }

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (child.name == objectName)
                {
                    return child.GetComponent<Button>();
                }
            }
        }

        return null;
    }

    private void SpawnEnemy(EnemyDefinition definition, Vector3 position)
    {
        EnemyBase enemy = CreateEnemy(definition, position);
        enemy.Initialize(definition, currentWave);

        EnemyChaseAI chaseAI = enemy.GetComponent<EnemyChaseAI>();
        if (chaseAI == null)
        {
            chaseAI = enemy.gameObject.AddComponent<EnemyChaseAI>();
        }

        chaseAI.SetTarget(playerTarget);
        aliveEnemies.Add(enemy);
    }

    private EnemyBase CreateEnemy(EnemyDefinition definition, Vector3 position)
    {
        EnemyBase prefab = GetPrefabForEnemy(definition);
        if (prefab != null)
        {
            return Instantiate(prefab, position, Quaternion.identity);
        }

        GameObject enemyObject = new GameObject("Enemy");
        enemyObject.transform.position = position;

        SpriteRenderer spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetFallbackSprite();
        spriteRenderer.color = new Color(0.95f, 0.28f, 0.2f, 1f);
        enemyObject.transform.localScale = Vector3.one * 0.6f;

        Rigidbody2D rb = enemyObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        CircleCollider2D collider = enemyObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        return enemyObject.AddComponent<EnemyBase>();
    }

    private EnemyBase GetPrefabForEnemy(EnemyDefinition definition)
    {
        if (definition != null && enemyPrefabs != null)
        {
            foreach (EnemyPrefabBinding binding in enemyPrefabs)
            {
                if (binding == null || binding.prefab == null)
                {
                    continue;
                }

                if (string.Equals(binding.enemyId, definition.Id, StringComparison.OrdinalIgnoreCase))
                {
                    return binding.prefab;
                }
            }
        }

        return defaultEnemyPrefab;
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 center = playerTarget != null ? playerTarget.position : transform.position;
        for (int attempt = 0; attempt < spawnPositionAttempts; attempt++)
        {
            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = Vector2.right;
            }

            Vector3 candidate = center + (Vector3)(direction * spawnRadius);
            if (!restrictSpawnToMapBounds || IsInsideMapBounds(candidate))
            {
                return candidate;
            }
        }

        Vector2 fallbackDirection = UnityEngine.Random.insideUnitCircle.normalized;
        if (fallbackDirection.sqrMagnitude <= 0.0001f)
        {
            fallbackDirection = Vector2.right;
        }

        return ClampToMapBounds(center + (Vector3)(fallbackDirection * spawnRadius));
    }

    private bool IsInsideMapBounds(Vector3 position)
    {
        if (!restrictSpawnToMapBounds)
        {
            return true;
        }

        Rect bounds = GetMapBoundsRect();
        return position.x >= bounds.xMin
            && position.x <= bounds.xMax
            && position.y >= bounds.yMin
            && position.y <= bounds.yMax;
    }

    private Vector3 ClampToMapBounds(Vector3 position)
    {
        if (!restrictSpawnToMapBounds)
        {
            return position;
        }

        Rect bounds = GetMapBoundsRect();
        position.x = Mathf.Clamp(position.x, bounds.xMin, bounds.xMax);
        position.y = Mathf.Clamp(position.y, bounds.yMin, bounds.yMax);
        return position;
    }

    private Rect GetMapBoundsRect()
    {
        if (mapBoundsCollider != null)
        {
            Bounds bounds = mapBoundsCollider.bounds;
            float minX = bounds.min.x + mapBoundsPadding;
            float minY = bounds.min.y + mapBoundsPadding;
            float maxX = bounds.max.x - mapBoundsPadding;
            float maxY = bounds.max.y - mapBoundsPadding;
            if (maxX < minX)
            {
                float centerX = bounds.center.x;
                minX = centerX;
                maxX = centerX;
            }

            if (maxY < minY)
            {
                float centerY = bounds.center.y;
                minY = centerY;
                maxY = centerY;
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        Vector2 halfSize = spawnAreaSize * 0.5f;
        float xMin = spawnAreaCenter.x - halfSize.x + mapBoundsPadding;
        float yMin = spawnAreaCenter.y - halfSize.y + mapBoundsPadding;
        float xMax = spawnAreaCenter.x + halfSize.x - mapBoundsPadding;
        float yMax = spawnAreaCenter.y + halfSize.y - mapBoundsPadding;
        if (xMax < xMin)
        {
            xMin = spawnAreaCenter.x;
            xMax = spawnAreaCenter.x;
        }

        if (yMax < yMin)
        {
            yMin = spawnAreaCenter.y;
            yMax = spawnAreaCenter.y;
        }

        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    private void TrimDeadEnemies()
    {
        for (int index = aliveEnemies.Count - 1; index >= 0; index--)
        {
            if (aliveEnemies[index] == null)
            {
                aliveEnemies.RemoveAt(index);
            }
        }
    }

    private void DestroyAliveEnemies()
    {
        for (int index = aliveEnemies.Count - 1; index >= 0; index--)
        {
            EnemyBase enemy = aliveEnemies[index];
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }

        aliveEnemies.Clear();
    }

    private void FindPlayerTarget()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            playerTarget = player.transform;
            return;
        }

        GameObject taggedPlayer = GameObject.FindGameObjectWithTag("Player");
        if (taggedPlayer != null)
        {
            playerTarget = taggedPlayer.transform;
        }
    }

    private void EnsureDefaultWaveSettings()
    {
        if (waveSettings != null && waveSettings.Count > 0)
        {
            return;
        }

        waveSettings = new List<EnemyWaveSpawnSettings>
        {
            new EnemyWaveSpawnSettings
            {
                startWave = 1,
                useCatalogWaveDuration = true,
                enemyRules = new List<EnemyWaveEnemySpawnRule>
                {
                    new EnemyWaveEnemySpawnRule
                    {
                        enemyId = "enemy.baby_alien",
                        baseSpawnInterval = 1.1f,
                        intervalMultiplierOverWaveProgress = new AnimationCurve(
                            new Keyframe(0f, 1f),
                            new Keyframe(0.55f, 0.82f),
                            new Keyframe(1f, 0.55f)),
                        minimumSpawnInterval = 0.4f,
                        batchMode = EnemySpawnBatchMode.Single,
                        singleBatchMin = 1,
                        singleBatchMax = 2
                    }
                }
            },
            new EnemyWaveSpawnSettings
            {
                startWave = 2,
                useCatalogWaveDuration = true,
                enemyRules = new List<EnemyWaveEnemySpawnRule>
                {
                    new EnemyWaveEnemySpawnRule
                    {
                        enemyId = "enemy.baby_alien",
                        baseSpawnInterval = 0.95f,
                        intervalMultiplierOverWaveProgress = new AnimationCurve(
                            new Keyframe(0f, 1f),
                            new Keyframe(0.5f, 0.75f),
                            new Keyframe(1f, 0.45f)),
                        minimumSpawnInterval = 0.32f,
                        batchMode = EnemySpawnBatchMode.Single,
                        singleBatchMin = 1,
                        singleBatchMax = 2
                    },
                    new EnemyWaveEnemySpawnRule
                    {
                        enemyId = "enemy.chaser",
                        baseSpawnInterval = 2.2f,
                        intervalMultiplierOverWaveProgress = new AnimationCurve(
                            new Keyframe(0f, 1f),
                            new Keyframe(0.65f, 0.7f),
                            new Keyframe(1f, 0.5f)),
                        minimumSpawnInterval = 0.75f,
                        batchMode = EnemySpawnBatchMode.Group,
                        groupBatchMin = 3,
                        groupBatchMax = 6,
                        groupSpreadRadius = 1.5f
                    }
                }
            },
            new EnemyWaveSpawnSettings
            {
                startWave = 4,
                useCatalogWaveDuration = true,
                enemyRules = new List<EnemyWaveEnemySpawnRule>
                {
                    new EnemyWaveEnemySpawnRule
                    {
                        enemyId = "enemy.baby_alien",
                        baseSpawnInterval = 0.8f,
                        intervalMultiplierOverWaveProgress = new AnimationCurve(
                            new Keyframe(0f, 1f),
                            new Keyframe(0.4f, 0.7f),
                            new Keyframe(1f, 0.38f)),
                        minimumSpawnInterval = 0.24f,
                        batchMode = EnemySpawnBatchMode.Single,
                        singleBatchMin = 1,
                        singleBatchMax = 3
                    },
                    new EnemyWaveEnemySpawnRule
                    {
                        enemyId = "enemy.chaser",
                        baseSpawnInterval = 1.8f,
                        intervalMultiplierOverWaveProgress = new AnimationCurve(
                            new Keyframe(0f, 1f),
                            new Keyframe(0.55f, 0.75f),
                            new Keyframe(1f, 0.48f)),
                        minimumSpawnInterval = 0.55f,
                        batchMode = EnemySpawnBatchMode.Group,
                        groupBatchMin = 4,
                        groupBatchMax = 8,
                        groupSpreadRadius = 1.8f
                    },
                    new EnemyWaveEnemySpawnRule
                    {
                        enemyId = "enemy.spitter",
                        baseSpawnInterval = 3.5f,
                        intervalMultiplierOverWaveProgress = new AnimationCurve(
                            new Keyframe(0f, 1f),
                            new Keyframe(0.7f, 0.85f),
                            new Keyframe(1f, 0.65f)),
                        minimumSpawnInterval = 1.2f,
                        batchMode = EnemySpawnBatchMode.Mixed,
                        singleBatchMin = 1,
                        singleBatchMax = 1,
                        groupBatchMin = 2,
                        groupBatchMax = 3,
                        groupChance = 0.25f,
                        groupSpreadRadius = 1.2f
                    }
                }
            }
        };
    }

    private void ValidateWaveSettings()
    {
        if (waveSettings == null)
        {
            return;
        }

        foreach (EnemyWaveSpawnSettings settings in waveSettings)
        {
            if (settings == null)
            {
                continue;
            }

            settings.startWave = Mathf.Max(1, settings.startWave);
            settings.waveDurationSeconds = Mathf.Max(1f, settings.waveDurationSeconds);

            if (settings.enemyRules == null)
            {
                settings.enemyRules = new List<EnemyWaveEnemySpawnRule>();
                continue;
            }

            foreach (EnemyWaveEnemySpawnRule rule in settings.enemyRules)
            {
                if (rule == null)
                {
                    continue;
                }

                rule.baseSpawnInterval = Mathf.Max(0.05f, rule.baseSpawnInterval);
                rule.minimumSpawnInterval = Mathf.Clamp(rule.minimumSpawnInterval, 0.05f, rule.baseSpawnInterval);
                rule.singleBatchMin = Mathf.Max(1, rule.singleBatchMin);
                rule.singleBatchMax = Mathf.Max(rule.singleBatchMin, rule.singleBatchMax);
                rule.groupBatchMin = Mathf.Max(1, rule.groupBatchMin);
                rule.groupBatchMax = Mathf.Max(rule.groupBatchMin, rule.groupBatchMax);
                rule.groupSpreadRadius = Mathf.Max(0f, rule.groupSpreadRadius);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!restrictSpawnToMapBounds)
        {
            return;
        }

        Rect bounds = GetMapBoundsRect();
        Vector3 center = new Vector3(bounds.center.x, bounds.center.y, 0f);
        Vector3 size = new Vector3(bounds.width, bounds.height, 0f);

        Gizmos.color = new Color(0.2f, 0.85f, 0.35f, 0.9f);
        Gizmos.DrawWireCube(center, size);
    }

    private static float EvaluateMultiplier(AnimationCurve curve, float value)
    {
        if (curve == null || curve.length == 0)
        {
            return 1f;
        }

        return Mathf.Max(0f, curve.Evaluate(value));
    }

    private static Sprite GetFallbackSprite()
    {
        if (fallbackSprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            fallbackSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                texture.width);
        }

        return fallbackSprite;
    }

    private sealed class EnemySpawnRuntimeState
    {
        public EnemySpawnRuntimeState(
            EnemyDefinition definition,
            EnemyWaveEnemySpawnRule rule,
            int waveOffset)
        {
            this.definition = definition;
            this.rule = rule;
            this.waveOffset = waveOffset;
            nextSpawnTime = 0f;
        }

        public readonly EnemyDefinition definition;
        public readonly EnemyWaveEnemySpawnRule rule;
        public float nextSpawnTime;
        public int waveOffset;
    }
}
