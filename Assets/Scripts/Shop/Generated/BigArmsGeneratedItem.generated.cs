using System.Collections.Generic;

public sealed class BigArmsGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 12f, false),
        new ItemStatModifier("Ranged Damage", 6f, false),
        new ItemStatModifier("Attack Speed", -3f, true),
        new ItemStatModifier("Speed", -3f, true),
        new ItemStatModifier("Knockback", 3f, false),
    };

    public override string Id => "item.big_arms";
    public override string DisplayName => "Big Arms";
    public override string Description => "+12 Melee Damage +6 Ranged Damage +3 Knockback -3 % Attack Speed -3 % Speed";
    public override int BasePrice => 105;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
