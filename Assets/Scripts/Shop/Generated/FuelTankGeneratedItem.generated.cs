using System.Collections.Generic;

public sealed class FuelTankGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", -1f, false),
        new ItemStatModifier("Ranged Damage", -1f, false),
        new ItemStatModifier("Elemental Damage", 4f, false),
    };

    public override string Id => "item.fuel_tank";
    public override string DisplayName => "Fuel Tank";
    public override string Description => "+4 Elemental Damage -1 Melee Damage -1 Ranged Damage";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
