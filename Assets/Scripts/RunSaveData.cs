using System;
using System.Collections.Generic;

public enum RunSavePhase
{
    Combat,
    PostWave,
    Shop
}

[Serializable]
public sealed class RunStatSaveEntry
{
    public PlayerStatId statId;
    public int value;
}

[Serializable]
public sealed class RunPurchaseSaveEntry
{
    public string contentId;
    public int count;
}

[Serializable]
public sealed class ShopRunSaveData
{
    public List<string> offerIds = new List<string>();
    public List<bool> lockedOffers = new List<bool>();
    public List<RunPurchaseSaveEntry> purchaseCounts = new List<RunPurchaseSaveEntry>();
    public int paidRefreshCount;
    public int freeRefreshesUsed;
}

[Serializable]
public sealed class RunSaveData
{
    public const int CurrentVersion = 1;

    public int version = CurrentVersion;
    public RunSavePhase phase = RunSavePhase.Combat;
    public int wave = 1;
    public int totalEnemiesKilled;
    public float elapsedRunSeconds;
    public int currentHealth = 1;
    public float playerPositionX;
    public float playerPositionY;
    public int coins;
    public int retainedMaterials;
    public float currentExperience;
    public int pendingCrates;
    public List<int> pendingUpgradeLevels = new List<int>();
    public List<RunStatSaveEntry> stats = new List<RunStatSaveEntry>();
    public List<string> weaponIds = new List<string>();
    public List<string> itemIds = new List<string>();
    public ShopRunSaveData shop = new ShopRunSaveData();
}
