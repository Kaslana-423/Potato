using System.Collections.Generic;

public sealed class PumpkinGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -2f, true),
        new ItemStatModifier("Piercing Damage", 15f, true),
    };

    public override string Id => "item.pumpkin";
    public override string DisplayName => "Pumpkin";
    public override string Description => "+15% Piercing Damage. Can't go above base damage -2 % Damage";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
