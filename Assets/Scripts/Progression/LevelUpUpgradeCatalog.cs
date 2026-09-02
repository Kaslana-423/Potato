using System;
using System.Collections.Generic;
using UnityEngine;

public readonly struct LevelUpUpgradeOption
{
    public LevelUpUpgradeOption(PlayerStatId statId, string displayName, int tier, int value)
    {
        StatId = statId;
        DisplayName = displayName;
        Tier = tier;
        Value = value;
    }

    public PlayerStatId StatId { get; }
    public string DisplayName { get; }
    public int Tier { get; }
    public int Value { get; }
}

public static class LevelUpUpgradeCatalog
{
    private sealed class Definition
    {
        public Definition(PlayerStatId statId, string displayName, params int[] values)
        {
            StatId = statId;
            DisplayName = displayName;
            Values = values;
        }

        public PlayerStatId StatId { get; }
        public string DisplayName { get; }
        public int[] Values { get; }
    }

    private static readonly Definition[] Definitions =
    {
        new Definition(PlayerStatId.MaxHp, "最大生命", 3, 6, 9, 12),
        new Definition(PlayerStatId.HpRegeneration, "生命恢复", 2, 3, 4, 5),
        new Definition(PlayerStatId.LifeSteal, "生命窃取", 1, 2, 3, 4),
        new Definition(PlayerStatId.Damage, "伤害", 5, 8, 12, 16),
        new Definition(PlayerStatId.MeleeDamage, "近战伤害", 2, 4, 6, 8),
        new Definition(PlayerStatId.RangedDamage, "远程伤害", 1, 2, 3, 4),
        new Definition(PlayerStatId.ElementalDamage, "元素伤害", 1, 2, 3, 4),
        new Definition(PlayerStatId.AttackSpeed, "攻击速度", 5, 10, 15, 20),
        new Definition(PlayerStatId.CritChance, "暴击率", 3, 5, 7, 9),
        new Definition(PlayerStatId.Engineering, "工程学", 2, 3, 4, 5),
        new Definition(PlayerStatId.Range, "攻击范围", 15, 30, 45, 60),
        new Definition(PlayerStatId.Armor, "护甲", 1, 2, 3, 4),
        new Definition(PlayerStatId.Dodge, "闪避", 3, 6, 9, 12),
        new Definition(PlayerStatId.Speed, "移动速度", 3, 6, 9, 12),
        new Definition(PlayerStatId.Luck, "幸运", 5, 10, 15, 20),
        new Definition(PlayerStatId.Harvesting, "收获", 5, 8, 10, 12),
    };

    public static IReadOnlyList<LevelUpUpgradeOption> GenerateOptions(int rewardLevel, int luck, int count = 4)
    {
        int optionCount = Mathf.Clamp(count, 1, Definitions.Length);
        var available = new List<Definition>(Definitions);
        var options = new List<LevelUpUpgradeOption>(optionCount);
        for (int index = 0; index < optionCount; index++)
        {
            int selectedIndex = UnityEngine.Random.Range(0, available.Count);
            Definition definition = available[selectedIndex];
            available.RemoveAt(selectedIndex);

            int tier = RollTier(rewardLevel, luck);
            options.Add(new LevelUpUpgradeOption(
                definition.StatId,
                definition.DisplayName,
                tier,
                definition.Values[tier - 1]));
        }

        return options;
    }

    public static Color GetTierColor(int tier)
    {
        switch (tier)
        {
            case 2:
                return new Color(0.26f, 0.78f, 0.38f, 1f);
            case 3:
                return new Color(0.29f, 0.53f, 0.96f, 1f);
            case 4:
                return new Color(0.74f, 0.35f, 0.95f, 1f);
            default:
                return new Color(0.34f, 0.36f, 0.4f, 1f);
        }
    }

    public static string GetTierLabel(int tier)
    {
        return $"Tier {Mathf.Clamp(tier, 1, 4)}";
    }

    private static int RollTier(int level, int luck)
    {
        int guaranteedTier = GetGuaranteedTier(level);
        if (guaranteedTier > 0)
        {
            return guaranteedTier;
        }

        float luckMultiplier = Mathf.Max(0f, 1f + luck / 100f);
        float tier4Chance = Mathf.Min(0.08f, Mathf.Max(0, level - 7) * 0.0023f * luckMultiplier);
        float tier3Chance = Mathf.Min(0.25f, Mathf.Max(0, level - 3) * 0.02f * luckMultiplier);
        float tier2Chance = Mathf.Min(0.60f, Mathf.Max(0, level - 1) * 0.06f * luckMultiplier);

        float roll = UnityEngine.Random.value;
        if (roll < tier4Chance)
        {
            return 4;
        }

        roll -= tier4Chance;
        if (roll < tier3Chance)
        {
            return 3;
        }

        roll -= tier3Chance;
        return roll < tier2Chance ? 2 : 1;
    }

    private static int GetGuaranteedTier(int level)
    {
        if (level >= 25 && level % 5 == 0)
        {
            return 4;
        }

        if (level == 10 || level == 15 || level == 20)
        {
            return 3;
        }

        if (level == 5)
        {
            return 2;
        }

        return level == 1 ? 1 : 0;
    }
}
