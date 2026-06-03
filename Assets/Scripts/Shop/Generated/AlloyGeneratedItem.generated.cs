using System.Collections.Generic;

public sealed class AlloyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 3f, false),
        new ItemStatModifier("Ranged Damage", 3f, false),
        new ItemStatModifier("Elemental Damage", 3f, false),
        new ItemStatModifier("Crit Chance", 5f, true),
        new ItemStatModifier("Engineering", 3f, false),
        new ItemStatModifier("Dodge", -6f, true),
    };

    public override string Id => "item.alloy";
    public override string DisplayName => "Alloy";
    public override string Description => "+3 Melee Damage +3 Ranged Damage +3 Elemental Damage +3 Engineering +5 % Crit Chance -6 % Dodge";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
