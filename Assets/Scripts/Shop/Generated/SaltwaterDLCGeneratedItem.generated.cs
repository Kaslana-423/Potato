using System.Collections.Generic;

public sealed class SaltwaterDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 2f, false),
        new ItemStatModifier("Elemental Damage", -1f, false),
        new ItemStatModifier("Attack Speed", 3f, true),
        new ItemStatModifier("Speed", 10f, true),
    };

    public override string Id => "item.saltwater_dlc";
    public override string DisplayName => "Saltwater (DLC)";
    public override string Description => "+2 Melee Damage +3 % Attack Speed +10 % Speed for 3 seconds when you take damage -1 Elemental Damage";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
