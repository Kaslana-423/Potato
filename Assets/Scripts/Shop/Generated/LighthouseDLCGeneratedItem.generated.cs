using System.Collections.Generic;

public sealed class LighthouseDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Engineering", 19f, false),
    };

    public override string Id => "item.lighthouse_dlc";
    public override string DisplayName => "Lighthouse (DLC)";
    public override string Description => "+20 Engineering -1 Engineering for every 1 Structure you have";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
