using System.Collections.Generic;

public sealed class BoilingWaterGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -1f, false),
        new ItemStatModifier("Elemental Damage", 2f, false),
    };

    public override string Id => "item.boiling_water";
    public override string DisplayName => "Boiling Water";
    public override string Description => "+2 Elemental Damage -1 Max HP";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
