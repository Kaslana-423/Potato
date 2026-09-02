using System;
using System.Collections.Generic;

public static class ShopContentCatalog
{
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
        if (content != null && ids.Add(content.Id))
        {
            contents.Add(content);
        }
    }
}
