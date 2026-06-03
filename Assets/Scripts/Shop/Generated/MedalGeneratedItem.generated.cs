using System.Collections.Generic;

public sealed class MedalGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 3f, false),
        new ItemStatModifier("Damage", 3f, true),
        new ItemStatModifier("Crit Chance", -4f, true),
        new ItemStatModifier("Armor", 1f, false),
        new ItemStatModifier("Speed", 3f, true),
    };

    public override string Id => "item.medal";
    public override string DisplayName => "Medal";
    public override string Description => "+3 Max HP +3 % Damage +1 Armor +3 % Speed -4 % Crit Chance";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
