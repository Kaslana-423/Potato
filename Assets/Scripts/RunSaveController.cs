using System;
using UnityEngine;

public sealed class RunSaveController : MonoBehaviour
{
    [SerializeField, Min(0.5f)] private float autosaveIntervalSeconds = 1.5f;

    private EnemySpawner spawner;
    private PlayerHealth playerHealth;
    private PlayerExperience playerExperience;
    private PlayerLootCrateInventory crateInventory;
    private PlayerWallet wallet;
    private WeaponBag weaponBag;
    private RelicBag relicBag;
    private ShopManager shopManager;
    private GameRunSettlementController settlementController;
    private float nextAutosaveAt;
    private bool suspended;

    public static RunSaveController GetOrCreate()
    {
        RunSaveController existing = FindObjectOfType<RunSaveController>(true);
        if (existing != null)
        {
            return existing;
        }

        GameObject controllerObject = new GameObject("RunSaveController");
        return controllerObject.AddComponent<RunSaveController>();
    }

    public void Bind(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
        ResolveReferences();
        nextAutosaveAt = Time.realtimeSinceStartup + autosaveIntervalSeconds;
    }

    public void SetSuspended(bool value)
    {
        suspended = value;
    }

    public void RestoreState(RunSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        ResolveReferences();
        if (PlayerStats.Instance != null && saveData.stats != null)
        {
            foreach (RunStatSaveEntry entry in saveData.stats)
            {
                if (entry != null)
                {
                    PlayerStats.Instance.SetStat(entry.statId, entry.value);
                }
            }
        }

        if (playerHealth != null)
        {
            playerHealth.SetCurrentHealth(Mathf.Clamp(saveData.currentHealth, 1, playerHealth.MaxHealth));
            Vector3 position = playerHealth.transform.position;
            position.x = saveData.playerPositionX;
            position.y = saveData.playerPositionY;
            playerHealth.transform.position = position;
        }

        wallet?.SetCoins(saveData.coins);
        wallet?.SetRetainedMaterials(saveData.retainedMaterials);
        playerExperience?.RestoreState(saveData.currentExperience, saveData.pendingUpgradeLevels);
        crateInventory?.SetPendingCrates(saveData.pendingCrates);
        weaponBag?.RestoreContentIds(saveData.weaponIds);
        relicBag?.RestoreContentIds(saveData.itemIds);
        shopManager?.RestoreRunSaveState(saveData.shop);
        settlementController?.RestoreElapsedRunSeconds(saveData.elapsedRunSeconds);
    }

    public void RestoreShopState(ShopRunSaveData saveData)
    {
        ResolveReferences();
        shopManager?.RestoreRunSaveState(saveData);
    }

    public void SaveNow(RunSavePhase phase)
    {
        if (suspended || spawner == null || spawner.HasRunEnded)
        {
            return;
        }

        ResolveReferences();
        var saveData = new RunSaveData
        {
            characterId = GameSessionState.CurrentCharacterId,
            phase = phase,
            wave = spawner.CurrentWave,
            totalEnemiesKilled = spawner.TotalEnemiesKilled,
            elapsedRunSeconds = settlementController != null ? settlementController.ElapsedRunSeconds : 0f,
            currentHealth = playerHealth != null ? playerHealth.CurrentHealth : 1,
            playerPositionX = playerHealth != null ? playerHealth.transform.position.x : 0f,
            playerPositionY = playerHealth != null ? playerHealth.transform.position.y : 0f,
            coins = wallet != null ? wallet.Coins : 0,
            retainedMaterials = wallet != null ? wallet.RetainedMaterials : 0,
            currentExperience = playerExperience != null ? playerExperience.CurrentExperience : 0f,
            pendingCrates = crateInventory != null ? crateInventory.PendingCrates : 0,
            shop = shopManager != null ? shopManager.CaptureRunSaveState() : new ShopRunSaveData()
        };

        if (PlayerStats.Instance != null)
        {
            foreach (PlayerStatId statId in Enum.GetValues(typeof(PlayerStatId)))
            {
                saveData.stats.Add(new RunStatSaveEntry
                {
                    statId = statId,
                    value = PlayerStats.Instance.GetStat(statId)
                });
            }
        }

        if (playerExperience != null)
        {
            for (int index = 0; index < playerExperience.PendingUpgradeLevels.Count; index++)
            {
                saveData.pendingUpgradeLevels.Add(playerExperience.PendingUpgradeLevels[index]);
            }
        }

        AddContentIds(weaponBag, saveData.weaponIds);
        AddContentIds(relicBag, saveData.itemIds);
        GameSessionState.SaveRun(saveData);
        nextAutosaveAt = Time.realtimeSinceStartup + autosaveIntervalSeconds;
    }

    private void Update()
    {
        if (suspended
            || spawner == null
            || spawner.HasRunEnded
            || spawner.IsLevelRunning
            || spawner.IsStartingNextWave
            || Time.realtimeSinceStartup < nextAutosaveAt)
        {
            return;
        }

        SaveNow(spawner.CurrentSavePhase);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused
            && spawner != null
            && !spawner.IsLevelRunning
            && !spawner.IsStartingNextWave)
        {
            SaveNow(spawner.CurrentSavePhase);
        }
    }

    private void OnApplicationQuit()
    {
        if (spawner != null
            && !spawner.IsLevelRunning
            && !spawner.IsStartingNextWave)
        {
            SaveNow(spawner.CurrentSavePhase);
        }
    }

    private void ResolveReferences()
    {
        if (PlayerStats.Instance != null)
        {
            playerHealth = PlayerStats.Instance.GetComponent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>(true);
        }

        wallet = PlayerWallet.GetOrCreate();
        playerExperience = PlayerExperience.GetOrCreate();
        crateInventory = PlayerLootCrateInventory.GetOrCreate();
        if (weaponBag == null)
        {
            weaponBag = FindObjectOfType<WeaponBag>(true);
        }

        weaponBag?.EnsureStartingWeapon();

        if (relicBag == null)
        {
            relicBag = FindObjectOfType<RelicBag>(true);
        }

        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>(true);
        }

        if (settlementController == null)
        {
            settlementController = FindObjectOfType<GameRunSettlementController>(true);
        }
    }

    private static void AddContentIds(ShopBagBase bag, System.Collections.Generic.ICollection<string> destination)
    {
        if (bag == null || destination == null)
        {
            return;
        }

        foreach (ShopContentDefinition content in bag.Contents)
        {
            if (content != null && !string.IsNullOrWhiteSpace(content.Id))
            {
                destination.Add(content.Id);
            }
        }
    }
}
