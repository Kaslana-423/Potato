using System;
using System.Collections.Generic;
using UnityEngine;

public enum ShopContentKind
{
    Weapon,
    Item
}

public enum ShopRarity
{
    Tier1 = 1,
    Tier2 = 2,
    Tier3 = 3,
    Tier4 = 4
}

public abstract class ShopContentDefinition
{
    private Sprite cachedIcon;
    private bool iconLoaded;

    public abstract string Id { get; }
    public abstract string DisplayName { get; }
    public abstract ShopContentKind Kind { get; }
    public abstract int BasePrice { get; }
    public abstract ShopRarity Rarity { get; }

    public virtual string Description => string.Empty;
    public virtual string IconResourcePath => string.Empty;
    public virtual IReadOnlyList<string> Tags => Array.Empty<string>();

    public string LocalizedDisplayName => ShopLocalization.GetContentName(Id, DisplayName);
    public string LocalizedDescription => ShopLocalization.GetContentDescription(Id, Description);
    public string RarityLabel => ShopLocalization.GetRarityLabel(Rarity);

    public Sprite LoadIcon()
    {
        if (iconLoaded)
        {
            return cachedIcon;
        }

        iconLoaded = true;
        if (!string.IsNullOrWhiteSpace(IconResourcePath))
        {
            cachedIcon = Resources.Load<Sprite>(IconResourcePath);
        }

        return cachedIcon;
    }

    public virtual string BuildStatLine()
    {
        return string.Empty;
    }

    public virtual string BuildDetails()
    {
        string statLine = BuildStatLine();
        return string.IsNullOrWhiteSpace(statLine)
            ? LocalizedDescription
            : $"{LocalizedDescription}\n{statLine}";
    }
}
