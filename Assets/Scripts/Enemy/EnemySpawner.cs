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

    [Header("Run Completion")]
    [SerializeField] private bool endRunAfterFinalWave = true;
    [SerializeField, Min(1)] private int finalWave = 20;

    [Header("Shop Flow")]
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private GameObject shopRoot;
    [SerializeField] private Button shopExitButton;
    [SerializeField, Min(0f)] private float shopOpenDelaySeconds = 1f;
    [SerializeField, Min(0f)] private float nextLevelDelaySeconds = 0.5f;
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
    private PlayerExperience playerExperience;
    private LevelUpRewardController levelUpRewardController;
    private LootCrateRewardController lootCrateRewardController;
    private GameRunSettlementController settlementController;
    private RunSaveController saveController;
    private Coroutine spawnRoutine;
    private Coroutine nextLevelRoutine;
    private bool levelRunning;
    private int levelRunId;
    private float currentWaveDurationSeconds;
    private float currentWaveStartTime;
    private float lastWaveElapsedSeconds;
    private int totalEnemiesKilled;
    private bool runEnded;

    public int CurrentWave => currentWave;
    public int AliveCount => lifetimeTracker != null ? lifetimeTracker.AliveCount : 0;
    public bool IsLevelRunning => levelRunning;
    public int TotalEnemiesKilled => totalEnemiesKilled;
    public int FinalWave => finalWave;
    public bool HasRunEnded => runEnded;
    public bool IsStartingNextWave => nextLevelRoutine != null;
    public RunSavePhase CurrentSavePhase => levelRunning
        ? RunSavePhase.Combat
        : shopManager != null && shopManager.IsOpen
            ? RunSavePhase.Shop
            : RunSavePhase.PostWave;
    public float CurrentWaveDurationSeconds => currentWaveDurationSeconds > 0f
        ? currentWaveDurationSeconds
        : GetWaveDuration(currentWave);
    public float CurrentWaveElapsedSeconds => levelRunning
        ? Mathf.Clamp(Time.time - currentWaveStartTime, 0f, CurrentWaveDurationSeconds)
        : Mathf.Clamp(lastWaveElapsedSeconds, 0f, CurrentWaveDurationSeconds);
    public float CurrentWaveRemainingSeconds => Mathf.Max(0f, CurrentWaveDurationSeconds - CurrentWaveElapsedSeconds);

    private void Reset()
    {
        EnsureDefaultWaveSettings();
    }

    private void OnValidate()
    {
        currentWave = Mathf.Max(1, currentWave);
        finalWave = Mathf.Max(1, finalWave);
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
        PrewarmEnemyPools();
        EnsureLifetimeTracker();
        EnsureShopFlow();
        EnsureLevelUpFlow();
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

        EnsureSettlementFlow();
        EnsureSaveFlow();
        GameplayPauseController.FindSceneController(this);

        if (GameSessionState.TryLoadRun(out RunSaveData saveData))
        {
            saveController.SetSuspended(true);
            StartCoroutine(RestoreSavedRunRoutine(saveData));
        }
        else if (spawnOnStart)
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
        if (runEnded)
        {
            return;
        }

        StopSpawning();
        levelRunId++;
        levelRunning = true;
        spawnRoutine = StartCoroutine(SpawnWaveLoop());
        saveController?.SaveNow(RunSavePhase.Combat);
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
        currentWaveDurationSeconds = GetWaveDuration(currentWave);
        lastWaveElapsedSeconds = 0f;
    }

    public void StartWave(int wave)
    {
        SetWave(wave);
        SetShopVisible(false);
        StartSpawning();
    }

    public void ExitShopAndStartNextLevel()
    {
        if (runEnded)
        {
            return;
        }

        if (nextLevelRoutine != null)
        {
            StopCoroutine(nextLevelRoutine);
        }

        saveController?.SaveNow(RunSavePhase.Shop);
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
        float waveDuration = GetWaveDuration(currentWave);
        BeginWaveTimer(waveDuration);

        if (states.Count == 0)
        {
            yield return FinishWaveAndOpenShop(runId);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < waveDuration)
        {
            elapsed = CurrentWaveElapsedSeconds;
            if (elapsed >= waveDuration)
            {
                break;
            }

            TrimDeadEnemies();

            EnemyWaveSpawnRuntimeState readyState = EnemyWaveSpawnPlanner.GetNextReadyState(states, elapsed);
            if (readyState == null)
            {
                float waitTime = Mathf.Min(EnemyWaveSpawnPlanner.GetTimeUntilNextSpawn(states, elapsed), waveDuration - elapsed);
                yield return new WaitForSeconds(waitTime);
                elapsed += waitTime;
                lastWaveElapsedSeconds = elapsed;
                continue;
            }

            float progress = waveDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / waveDuration);
            ScheduleSpawnBatch(readyState, runId);
            readyState.nextSpawnTime = elapsed + EnemyWaveSpawnPlanner.GetSpawnInterval(readyState, progress);

            yield return null;
            lastWaveElapsedSeconds = CurrentWaveElapsedSeconds;
        }

        yield return FinishWaveAndOpenShop(runId);
    }

    private IEnumerator FinishWaveAndOpenShop(int runId)
    {
        lastWaveElapsedSeconds = CurrentWaveDurationSeconds;
        levelRunning = false;
        spawnRoutine = null;
        if (clearAliveEnemiesWhenLevelEnds)
        {
            DestroyAliveEnemies();
        }

        RefillPlayerHealth();
        StoreAndClearBattlefieldDrops();
        saveController?.SaveNow(RunSavePhase.PostWave);

        if (shopOpenDelaySeconds > 0f)
        {
            yield return new WaitForSeconds(shopOpenDelaySeconds);
        }

        if (runId == levelRunId && endRunAfterFinalWave && currentWave >= finalWave)
        {
            EnsureSettlementFlow();
            settlementController?.ShowVictory();
            yield break;
        }

        yield return ProcessPostWaveRewardsAndOpenShop(runId);
    }

    private IEnumerator ProcessPostWaveRewardsAndOpenShop(int runId)
    {
        if (runId == levelRunId
            && playerExperience != null
            && playerExperience.PendingUpgradeCount > 0)
        {
            EnsureLevelUpFlow();
            levelUpRewardController.BeginRewards(playerExperience, currentWave);
            while (runId == levelRunId
                && levelUpRewardController != null
                && levelUpRewardController.IsProcessing)
            {
                yield return null;
            }
        }

        PlayerLootCrateInventory crateInventory = PlayerLootCrateInventory.GetOrCreate();
        if (runId == levelRunId
            && crateInventory != null
            && crateInventory.PendingCrates > 0)
        {
            EnsureLevelUpFlow();
            EnsureShopFlow();
            shopFlow.AutoBind(ref shopManager, shopRoot, ref shopExitButton);
            lootCrateRewardController.BeginRewards(crateInventory, shopManager);
            while (runId == levelRunId
                && lootCrateRewardController != null
                && lootCrateRewardController.IsProcessing)
            {
                yield return null;
            }
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
        batchSize = ApplyEnemyCountModifier(batchSize);

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

    private static int ApplyEnemyCountModifier(int baseCount)
    {
        int enemyCountModifier = PlayerStats.Instance != null ? PlayerStats.Instance.Enemies : 0;
        float scaledCount = Mathf.Max(0f, baseCount * (1f + enemyCountModifier / 100f));
        int wholeEnemies = Mathf.FloorToInt(scaledCount);
        float fractionalEnemy = scaledCount - wholeEnemies;
        return wholeEnemies + (UnityEngine.Random.value < fractionalEnemy ? 1 : 0);
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

    private void BeginWaveTimer(float waveDuration)
    {
        currentWaveDurationSeconds = Mathf.Max(0f, waveDuration);
        currentWaveStartTime = Time.time;
        lastWaveElapsedSeconds = 0f;
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
        saveController?.SaveNow(RunSavePhase.Shop);
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

    public void EndRunCombat()
    {
        if (runEnded)
        {
            return;
        }

        runEnded = true;
        levelRunId++;
        StopSpawning();
        SetShopVisible(false);
        DestroyAliveEnemies();
        ClearBattlefieldDrops();
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

    private void RefillPlayerHealth()
    {
        PlayerHealth playerHealth = playerTarget != null
            ? playerTarget.GetComponentInParent<PlayerHealth>()
            : null;

        if (playerHealth == null && PlayerStats.Instance != null)
        {
            playerHealth = PlayerStats.Instance.GetComponent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        playerHealth?.Refill();
    }

    private void StoreAndClearBattlefieldDrops()
    {
        BattlefieldDrop[] drops = FindObjectsOfType<BattlefieldDrop>(true);
        int retainedMaterialUnits = 0;
        int uncollectedLootCrates = 0;
        foreach (BattlefieldDrop drop in drops)
        {
            if (drop == null || !drop.gameObject.scene.IsValid())
            {
                continue;
            }

            CoinPickup material = drop as CoinPickup;
            if (material != null)
            {
                retainedMaterialUnits += material.RetainedMaterialUnits;
            }
            else if (drop is LootCratePickup)
            {
                uncollectedLootCrates++;
            }

            drop.gameObject.SetActive(false);
            Destroy(drop.gameObject);
        }

        if (retainedMaterialUnits > 0)
        {
            PlayerWallet.GetOrCreate().AddRetainedMaterials(retainedMaterialUnits);
        }

        if (uncollectedLootCrates > 0)
        {
            PlayerLootCrateInventory.GetOrCreate()?.AddCrates(uncollectedLootCrates);
        }
    }

    private static void ClearBattlefieldDrops()
    {
        BattlefieldDrop[] drops = FindObjectsOfType<BattlefieldDrop>(true);
        foreach (BattlefieldDrop drop in drops)
        {
            if (drop == null || !drop.gameObject.scene.IsValid())
            {
                continue;
            }

            drop.gameObject.SetActive(false);
            Destroy(drop.gameObject);
        }
    }

    private void PrewarmEnemyPools()
    {
        if (spawnPool == null)
        {
            return;
        }

        spawnPool.Prewarm(defaultEnemyPrefab);
        if (enemyPrefabs == null)
        {
            return;
        }

        foreach (EnemyPrefabBinding binding in enemyPrefabs)
        {
            if (binding != null && binding.prefab != null)
            {
                spawnPool.Prewarm(binding.prefab);
            }
        }
    }

    private void EnsureLifetimeTracker()
    {
        if (lifetimeTracker == null)
        {
            lifetimeTracker = new EnemyLifetimeTracker(ReleaseOrDestroyEnemy, HandleEnemyDied);
        }
    }

    private void HandleEnemyDied(EnemyBase enemy)
    {
        totalEnemiesKilled++;
    }

    private void EnsureSettlementFlow()
    {
        if (settlementController == null)
        {
            settlementController = GameRunSettlementController.GetOrCreate();
        }

        PlayerHealth playerHealth = playerTarget != null
            ? playerTarget.GetComponentInParent<PlayerHealth>()
            : null;
        if (playerHealth == null && PlayerStats.Instance != null)
        {
            playerHealth = PlayerStats.Instance.GetComponent<PlayerHealth>();
        }

        settlementController?.Bind(this, playerHealth);
    }

    private void EnsureSaveFlow()
    {
        if (saveController == null)
        {
            saveController = RunSaveController.GetOrCreate();
        }

        saveController.Bind(this);
    }

    private IEnumerator RestoreSavedRunRoutine(RunSaveData saveData)
    {
        yield return null;

        SetWave(saveData.wave);
        totalEnemiesKilled = Mathf.Max(0, saveData.totalEnemiesKilled);
        saveController.RestoreState(saveData);

        if (saveData.phase == RunSavePhase.Shop)
        {
            OpenShop();
            saveController.RestoreShopState(saveData.shop);
            saveController.SetSuspended(false);
            saveController.SaveNow(RunSavePhase.Shop);
            yield break;
        }

        if (saveData.phase == RunSavePhase.PostWave)
        {
            levelRunId++;
            int runId = levelRunId;
            saveController.SetSuspended(false);
            saveController.SaveNow(RunSavePhase.PostWave);
            if (endRunAfterFinalWave && currentWave >= finalWave)
            {
                settlementController?.ShowVictory();
                yield break;
            }

            yield return ProcessPostWaveRewardsAndOpenShop(runId);
            yield break;
        }

        saveController.SetSuspended(false);
        StartSpawning();
    }

    private void EnsureShopFlow()
    {
        if (shopFlow == null)
        {
            shopFlow = new EnemyShopFlow(ExitShopAndStartNextLevel);
        }
    }

    private void EnsureLevelUpFlow()
    {
        if (playerExperience == null)
        {
            playerExperience = PlayerExperience.GetOrCreate();
        }

        if (playerExperience != null)
        {
            PlayerExperienceHudView.GetOrCreate(playerExperience);
        }

        if (levelUpRewardController == null)
        {
            levelUpRewardController = LevelUpRewardController.GetOrCreate();
        }

        if (lootCrateRewardController == null)
        {
            lootCrateRewardController = LootCrateRewardController.GetOrCreate();
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
