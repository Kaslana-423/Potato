using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyWaveSpawnPlanner
{
    public static List<EnemyWaveSpawnRuntimeState> BuildSpawnStates(
        int wave,
        bool useConfiguredWaves,
        List<EnemyWaveSpawnSettings> waveSettings,
        float fallbackSpawnInterval,
        int fallbackEnemiesPerSpawn,
        bool includeDlcEnemies,
        bool includeElites,
        bool includeBosses)
    {
        EnemyWaveSpawnSettings settings = GetWaveSettings(wave, useConfiguredWaves, waveSettings);
        if (settings == null || settings.enemyRules == null || settings.enemyRules.Count == 0)
        {
            return BuildFallbackSpawnStates(
                wave,
                fallbackSpawnInterval,
                fallbackEnemiesPerSpawn,
                includeDlcEnemies,
                includeElites,
                includeBosses);
        }

        int waveOffset = Mathf.Max(0, wave - Mathf.Max(1, settings.startWave));
        var states = new List<EnemyWaveSpawnRuntimeState>();
        foreach (EnemyWaveEnemySpawnRule rule in settings.enemyRules)
        {
            if (rule == null || !EnemyCatalog.TryGetById(rule.enemyId, out EnemyDefinition enemy))
            {
                continue;
            }

            if (!CanSpawnEnemy(enemy, wave, includeDlcEnemies, includeElites, includeBosses))
            {
                continue;
            }

            states.Add(new EnemyWaveSpawnRuntimeState(enemy, rule, waveOffset));
        }

        return states;
    }

    public static EnemyWaveSpawnSettings GetWaveSettings(
        int wave,
        bool useConfiguredWaves,
        List<EnemyWaveSpawnSettings> waveSettings)
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

    public static float GetWaveDuration(
        int wave,
        bool useConfiguredWaves,
        List<EnemyWaveSpawnSettings> waveSettings)
    {
        EnemyWaveSpawnSettings settings = GetWaveSettings(wave, useConfiguredWaves, waveSettings);
        if (settings != null && !settings.useCatalogWaveDuration)
        {
            return Mathf.Max(1f, settings.waveDurationSeconds);
        }

        return EnemyCatalog.GetWaveDurationSeconds(wave);
    }

    public static EnemyDefinition PickFallbackEnemyDefinition(
        int wave,
        bool includeDlcEnemies,
        bool includeElites,
        bool includeBosses)
    {
        List<EnemyDefinition> eligibleEnemies = EnemyCatalog
            .GetEligibleEnemies(wave, includeDlcEnemies, includeElites, includeBosses)
            .ToList();

        if (eligibleEnemies.Count == 0)
        {
            return null;
        }

        return eligibleEnemies[Random.Range(0, eligibleEnemies.Count)];
    }

    public static float GetSpawnInterval(EnemyWaveSpawnRuntimeState state, float waveProgress)
    {
        float interval = state.rule.baseSpawnInterval;
        interval *= EvaluateMultiplier(state.rule.intervalMultiplierByWaveOffset, state.waveOffset);
        interval *= EvaluateMultiplier(state.rule.intervalMultiplierOverWaveProgress, waveProgress);
        return Mathf.Max(state.rule.minimumSpawnInterval, interval);
    }

    public static EnemyWaveSpawnRuntimeState GetNextReadyState(
        IReadOnlyList<EnemyWaveSpawnRuntimeState> states,
        float elapsed)
    {
        EnemyWaveSpawnRuntimeState selected = null;
        foreach (EnemyWaveSpawnRuntimeState state in states)
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

    public static float GetTimeUntilNextSpawn(
        IReadOnlyList<EnemyWaveSpawnRuntimeState> states,
        float elapsed)
    {
        float nextTime = float.PositiveInfinity;
        foreach (EnemyWaveSpawnRuntimeState state in states)
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

    public static List<EnemyWaveSpawnSettings> CreateDefaultWaveSettings()
    {
        return new List<EnemyWaveSpawnSettings>
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

    public static void ValidateWaveSettings(List<EnemyWaveSpawnSettings> waveSettings)
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

    private static List<EnemyWaveSpawnRuntimeState> BuildFallbackSpawnStates(
        int wave,
        float fallbackSpawnInterval,
        int fallbackEnemiesPerSpawn,
        bool includeDlcEnemies,
        bool includeElites,
        bool includeBosses)
    {
        List<EnemyDefinition> eligibleEnemies = EnemyCatalog
            .GetEligibleEnemies(wave, includeDlcEnemies, includeElites, includeBosses)
            .ToList();

        if (eligibleEnemies.Count == 0)
        {
            return new List<EnemyWaveSpawnRuntimeState>();
        }

        var states = new List<EnemyWaveSpawnRuntimeState>();

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

            states.Add(new EnemyWaveSpawnRuntimeState(enemy, rule, Mathf.Max(0, wave - 1)));
        }

        return states;
    }

    private static bool CanSpawnEnemy(
        EnemyDefinition enemy,
        int wave,
        bool includeDlcEnemies,
        bool includeElites,
        bool includeBosses)
    {
        return enemy != null
            && enemy.FirstWave <= wave
            && (includeDlcEnemies || !enemy.IsDlc)
            && (includeElites || !enemy.IsElite)
            && (includeBosses || !enemy.IsBoss);
    }

    private static float EvaluateMultiplier(AnimationCurve curve, float value)
    {
        if (curve == null || curve.length == 0)
        {
            return 1f;
        }

        return Mathf.Max(0f, curve.Evaluate(value));
    }
}
