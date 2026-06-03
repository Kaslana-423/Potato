using System.Collections.Generic;

public sealed class KnotDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 15f, false),
        new ItemStatModifier("Damage", 15f, true),
    };

    public override string Id => "item.knot_dlc";
    public override string DisplayName => "Knot (DLC)";
    public override string Description => "+15 % Damage +15 Max HP Weapons can no longer be upgraded or recycled";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
