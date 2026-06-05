using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private EnemySpawnPool spawnPool;
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

    private EnemyLifetimeTracker lifetimeTracker;
    private EnemyShopFlow shopFlow;
    private Coroutine spawnRoutine;
    private Coroutine nextLevelRoutine;
    private bool levelRunning;
    private int levelRunId;

    public int CurrentWave => currentWave;
    public int AliveCount => lifetimeTracker != null ? lifetimeTracker.AliveCount : 0;
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
        EnsureSpawnPool();
        EnsureLifetimeTracker();
        EnsureShopFlow();
        shopFlow.AutoBind(ref shopManager, shopRoot, ref shopExitButton);
        shopFlow.BindExitButton(shopExitButton);
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
        shopFlow?.UnbindExitButton();
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
        List<EnemyWaveSpawnRuntimeState> states = BuildSpawnStates(currentWave);
        if (states.Count == 0)
        {
            yield return FinishWaveAndOpenShop(runId);
            yield break;
        }

        float elapsed = 0f;
        float waveDuration = GetWaveDuration(currentWave);

        while (elapsed < waveDuration)
        {
            TrimDeadEnemies();

            EnemyWaveSpawnRuntimeState readyState = EnemyWaveSpawnPlanner.GetNextReadyState(states, elapsed);
            if (readyState == null)
            {
                float waitTime = Mathf.Min(EnemyWaveSpawnPlanner.GetTimeUntilNextSpawn(states, elapsed), waveDuration - elapsed);
                yield return new WaitForSeconds(waitTime);
                elapsed += waitTime;
                continue;
            }

            float progress = waveDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / waveDuration);
            ScheduleSpawnBatch(readyState, runId);
            readyState.nextSpawnTime = elapsed + EnemyWaveSpawnPlanner.GetSpawnInterval(readyState, progress);

            yield return null;
        }

        yield return FinishWaveAndOpenShop(runId);
    }

    private IEnumerator FinishWaveAndOpenShop(int runId)
    {
        levelRunning = false;
        spawnRoutine = null;
        if (clearAliveEnemiesWhenLevelEnds)
        {
            DestroyAliveEnemies();
        }

        if (shopOpenDelaySeconds > 0f)
        {
            yield return new WaitForSeconds(shopOpenDelaySeconds);
        }

        if (runId == levelRunId)
        {
            OpenShop();
        }
    }

    private List<EnemyWaveSpawnRuntimeState> BuildSpawnStates(int wave)
    {
        return EnemyWaveSpawnPlanner.BuildSpawnStates(
            wave,
            useConfiguredWaves,
            waveSettings,
            fallbackSpawnInterval,
            fallbackEnemiesPerSpawn,
            includeDlcEnemies,
            includeElites,
            includeBosses);
    }

    private int ScheduleSpawnBatch(EnemyWaveSpawnRuntimeState state, int runId)
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
            ? EnemySpawnPositionResolver.BuildGroupSpawnPositions(
                center,
                batchSize,
                state.rule.groupSpreadRadius,
                restrictSpawnToMapBounds,
                mapBoundsCollider,
                spawnAreaCenter,
                spawnAreaSize,
                mapBoundsPadding)
            : EnemySpawnPositionResolver.BuildSingleSpawnPositions(
                center,
                batchSize,
                restrictSpawnToMapBounds,
                mapBoundsCollider,
                spawnAreaCenter,
                spawnAreaSize,
                mapBoundsPadding);

        for (int index = 0; index < positions.Count; index++)
        {
            ShowSpawnWarning(positions[index]);
        }

        StartCoroutine(SpawnAfterWarning(state.definition, positions, runId));
        return batchSize;
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

    private float GetWaveDuration(int wave)
    {
        return EnemyWaveSpawnPlanner.GetWaveDuration(wave, useConfiguredWaves, waveSettings);
    }

    private EnemyDefinition PickFallbackEnemyDefinition()
    {
        return EnemyWaveSpawnPlanner.PickFallbackEnemyDefinition(
            currentWave,
            includeDlcEnemies,
            includeElites,
            includeBosses);
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
        EnsureShopFlow();
        shopFlow.Open(ref shopManager, shopRoot, ref shopExitButton, refreshShopWhenOpened);
    }

    private void SetShopVisible(bool visible)
    {
        EnsureShopFlow();
        shopFlow.SetVisible(ref shopManager, shopRoot, ref shopExitButton, visible);
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
        EnsureLifetimeTracker();
        lifetimeTracker.Track(enemy);
    }

    private EnemyBase CreateEnemy(EnemyDefinition definition, Vector3 position)
    {
        EnsureSpawnPool();
        if (spawnPool != null)
        {
            return spawnPool.Get(GetPrefabForEnemy(definition), position, Quaternion.identity);
        }

        EnemyBase prefab = GetPrefabForEnemy(definition);
        return prefab != null
            ? Instantiate(prefab, position, Quaternion.identity)
            : new GameObject("Enemy").AddComponent<EnemyBase>();
    }

    private void ReleaseOrDestroyEnemy(EnemyBase enemy)
    {
        if (enemy == null)
        {
            return;
        }

        if (spawnPool != null && spawnPool.Release(enemy))
        {
            return;
        }

        Destroy(enemy.gameObject);
    }

    private void EnsureSpawnPool()
    {
        if (spawnPool != null)
        {
            return;
        }

        spawnPool = GetComponent<EnemySpawnPool>();
        if (spawnPool == null)
        {
            spawnPool = gameObject.AddComponent<EnemySpawnPool>();
        }
    }

    private void EnsureLifetimeTracker()
    {
        if (lifetimeTracker == null)
        {
            lifetimeTracker = new EnemyLifetimeTracker(ReleaseOrDestroyEnemy);
        }
    }

    private void EnsureShopFlow()
    {
        if (shopFlow == null)
        {
            shopFlow = new EnemyShopFlow(ExitShopAndStartNextLevel);
        }
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
        return EnemySpawnPositionResolver.GetSpawnPosition(
            playerTarget,
            transform,
            spawnRadius,
            spawnPositionAttempts,
            restrictSpawnToMapBounds,
            mapBoundsCollider,
            spawnAreaCenter,
            spawnAreaSize,
            mapBoundsPadding);
    }

    private void TrimDeadEnemies()
    {
        lifetimeTracker?.TrimMissing();
    }

    private void DestroyAliveEnemies()
    {
        lifetimeTracker?.ReleaseAll();
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

        waveSettings = EnemyWaveSpawnPlanner.CreateDefaultWaveSettings();
    }

    private void ValidateWaveSettings()
    {
        EnemyWaveSpawnPlanner.ValidateWaveSettings(waveSettings);
    }

    private void OnDrawGizmosSelected()
    {
        if (!restrictSpawnToMapBounds)
        {
            return;
        }

        Rect bounds = EnemySpawnPositionResolver.GetMapBoundsRect(
            mapBoundsCollider,
            spawnAreaCenter,
            spawnAreaSize,
            mapBoundsPadding);
        Vector3 center = new Vector3(bounds.center.x, bounds.center.y, 0f);
        Vector3 size = new Vector3(bounds.width, bounds.height, 0f);

        Gizmos.color = new Color(0.2f, 0.85f, 0.35f, 0.9f);
        Gizmos.DrawWireCube(center, size);
    }

}
