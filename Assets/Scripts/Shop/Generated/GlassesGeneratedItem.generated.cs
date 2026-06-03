using System.Collections.Generic;

public sealed class GlassesGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Range", 20f, false),
    };

    public override string Id => "item.glasses";
    public override string DisplayName => "Glasses";
    public override string Description => "+20 Range";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
