using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public readonly struct ItemStatModifier
{
    public ItemStatModifier(string statName, float value, bool isPercent = false)
    {
        StatName = statName;
        Value = value;
        IsPercent = isPercent;
    }

    public string StatName { get; }
    public float Value { get; }
    public bool IsPercent { get; }

    public override string ToString()
    {
        string sign = Value >= 0f ? "+" : string.Empty;
        string suffix = IsPercent ? "%" : string.Empty;
        string color = Value >= 0f ? "#55E875" : "#FF6868";
        return string.Format(
            CultureInfo.InvariantCulture,
            "<color={0}>{1}{2:0.##}{3}</color> {4}",
            color,
            sign,
            Value,
            suffix,
            ShopLocalization.GetStatName(StatName));
    }
}

public abstract class ShopItemDefinition : ShopContentDefinition
{
    public sealed override ShopContentKind Kind => ShopContentKind.Item;

    public virtual int PurchaseLimit => 0;
    public virtual IReadOnlyList<ItemStatModifier> Modifiers => Array.Empty<ItemStatModifier>();

    public override string BuildStatLine()
    {
        return Modifiers.Count == 0
            ? string.Empty
            : string.Join("\n", Modifiers.Select(modifier => modifier.ToString()));
    }
}

public readonly struct ShopItemEffectResult
{
    public ShopItemEffectResult(int appliedModifierCount, IReadOnlyList<string> unsupportedStats, bool hasPlayerStats)
    {
        AppliedModifierCount = appliedModifierCount;
        UnsupportedStats = unsupportedStats ?? Array.Empty<string>();
        HasPlayerStats = hasPlayerStats;
    }

    public int AppliedModifierCount { get; }
    public IReadOnlyList<string> UnsupportedStats { get; }
    public bool HasPlayerStats { get; }
}

public static class ShopItemEffectApplier
{
    public static ShopItemEffectResult Apply(ShopItemDefinition item, PlayerStats playerStats)
    {
        if (item == null)
        {
            return new ShopItemEffectResult(0, Array.Empty<string>(), playerStats != null);
        }

        var unsupportedStats = new List<string>();
        if (playerStats == null)
        {
            foreach (ItemStatModifier modifier in item.Modifiers)
            {
                AddUnique(unsupportedStats, modifier.StatName);
            }

            return new ShopItemEffectResult(0, unsupportedStats, false);
        }

        int appliedCount = 0;
        foreach (ItemStatModifier modifier in item.Modifiers)
        {
            if (!PlayerStats.TryParseStatId(modifier.StatName, out PlayerStatId statId))
            {
                AddUnique(unsupportedStats, modifier.StatName);
                continue;
            }

            // 百分比属性在 PlayerStats 中本身以“百分点”整数保存，因此和固定值使用同一加法入口。
            playerStats.AddStat(statId, Mathf.RoundToInt(modifier.Value));
            appliedCount++;
        }

        return new ShopItemEffectResult(appliedCount, unsupportedStats, true);
    }

    private static void AddUnique(ICollection<string> values, string value)
    {
        string normalized = string.IsNullOrWhiteSpace(value) ? "未知属性" : value.Trim();
        if (!values.Contains(normalized))
        {
            values.Add(normalized);
        }
    }
}
