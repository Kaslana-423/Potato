using System.Collections.Generic;

public sealed class CapeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", 5f, true),
        new ItemStatModifier("Melee Damage", -2f, false),
        new ItemStatModifier("Ranged Damage", -2f, false),
        new ItemStatModifier("Elemental Damage", -2f, false),
        new ItemStatModifier("Dodge", 20f, true),
    };

    public override string Id => "item.cape";
    public override string DisplayName => "Cape";
    public override string Description => "+5 % Life Steal +20 % Dodge -2 Melee Damage -2 Ranged Damage -2 Elemental Damage";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
