using System;
using System.Collections.Generic;
using System.Text;

public static class ShopContentCatalog
{
    private static readonly string[] removedStatTokens =
    {
        "elementaldamage",
        "engineering",
        "harvesting",
        "harvest",
        "元素伤害",
        "工程学",
        "收获"
    };

    private static readonly IReadOnlyList<ShopContentDefinition> all = BuildCatalog();

    public static IReadOnlyList<ShopContentDefinition> All => all;

    public static ShopContentDefinition FindById(string contentId)
    {
        if (string.IsNullOrWhiteSpace(contentId))
        {
            return null;
        }

        for (int index = 0; index < all.Count; index++)
        {
            ShopContentDefinition content = all[index];
            if (content != null && string.Equals(content.Id, contentId, StringComparison.OrdinalIgnoreCase))
            {
                return content;
            }
        }

        return null;
    }

    private static IReadOnlyList<ShopContentDefinition> BuildCatalog()
    {
        var contents = new List<ShopContentDefinition>();
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddUnique(contents, ids, new AcidItem());
        AddUnique(contents, ids, new AdrenalineItem());
        AddUnique(contents, ids, new AlienBabyItem());
        AddUnique(contents, ids, new AlienMagicItem());
        AddUnique(contents, ids, new AlienTongueItem());

        foreach (ShopContentDefinition generatedContent in GeneratedShopContentCatalog.CreateAll())
        {
            AddUnique(contents, ids, generatedContent);
        }

        return contents;
    }

    private static void AddUnique(
        ICollection<ShopContentDefinition> contents,
        ISet<string> ids,
        ShopContentDefinition content)
    {
        if (content != null && IsSupportedRuntimeContent(content) && ids.Add(content.Id))
        {
            contents.Add(content);
        }
    }

    private static bool IsSupportedRuntimeContent(ShopContentDefinition content)
    {
        if (ContainsRemovedStat(content.Description))
        {
            return false;
        }

        if (content is ShopItemDefinition item)
        {
            IReadOnlyList<ItemStatModifier> modifiers = item.Modifiers;
            for (int index = 0; index < modifiers.Count; index++)
            {
                if (ContainsRemovedStat(modifiers[index].StatName))
                {
                    return false;
                }
            }
        }

        if (content is ShopWeaponDefinition weapon
            && (ContainsRemovedStat(weapon.DamageScalingStats)
                || ContainsRemovedStat(weapon.SpecialEffects)))
        {
            return false;
        }

        return true;
    }

    private static bool ContainsRemovedStat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalizedBuilder = new StringBuilder(value.Length);
        for (int index = 0; index < value.Length; index++)
        {
            char character = value[index];
            if (char.IsLetterOrDigit(character))
            {
                normalizedBuilder.Append(char.ToLowerInvariant(character));
            }
        }

        string normalized = normalizedBuilder.ToString();
        for (int index = 0; index < removedStatTokens.Length; index++)
        {
            if (normalized.Contains(removedStatTokens[index]))
            {
                return true;
            }
        }

        return false;
    }
}
