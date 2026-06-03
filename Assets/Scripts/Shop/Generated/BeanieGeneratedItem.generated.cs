using System.Collections.Generic;

public sealed class BeanieGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Range", -6f, false),
        new ItemStatModifier("Speed", 4f, true),
    };

    public override string Id => "item.beanie";
    public override string DisplayName => "Beanie";
    public override string Description => "+4 % Speed -6 Range";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
