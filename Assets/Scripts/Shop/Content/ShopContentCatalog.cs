using System;
using System.Collections.Generic;

public static class ShopContentCatalog
{
    private static readonly IReadOnlyList<ShopContentDefinition> all = BuildCatalog();

    public static IReadOnlyList<ShopContentDefinition> All => all;

    private static IReadOnlyList<ShopContentDefinition> BuildCatalog()
    {
        var contents = new List<ShopContentDefinition>();
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddUnique(contents, ids, new BrickDlcWeapon());
        AddUnique(contents, ids, new CactiClubWeapon());
        AddUnique(contents, ids, new ChopperWeapon());
        AddUnique(contents, ids, new ClawWeapon());
        AddUnique(contents, ids, new KnifeWeapon());

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
