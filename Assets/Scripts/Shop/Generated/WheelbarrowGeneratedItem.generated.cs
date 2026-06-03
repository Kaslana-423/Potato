using System.Collections.Generic;

public sealed class WheelbarrowGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Armor", -1f, false),
        new ItemStatModifier("Harvesting", 16f, false),
    };

    public override string Id => "item.wheelbarrow";
    public override string DisplayName => "Wheelbarrow";
    public override string Description => "+16 Harvesting -1 Armor";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
