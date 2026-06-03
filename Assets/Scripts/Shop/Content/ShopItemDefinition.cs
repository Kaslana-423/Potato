using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
