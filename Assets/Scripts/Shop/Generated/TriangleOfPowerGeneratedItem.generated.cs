using System.Collections.Generic;

public sealed class TriangleOfPowerGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 18f, true),
        new ItemStatModifier("Armor", 1f, false),
    };

    public override string Id => "item.triangle_of_power";
    public override string DisplayName => "Triangle of Power";
    public override string Description => "+20 % Damage +1 Armor -2 % Damage when you take damage until the end of the wave";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
