using System;
using System.Collections.Generic;
using UnityEngine;

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
