using System.Collections.Generic;

public sealed class HeavyBulletsGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 10f, true),
        new ItemStatModifier("Ranged Damage", 5f, false),
        new ItemStatModifier("Attack Speed", -5f, true),
        new ItemStatModifier("Crit Chance", -5f, true),
        new ItemStatModifier("Range", 10f, false),
    };

    public override string Id => "item.heavy_bullets";
    public override string DisplayName => "Heavy Bullets";
    public override string Description => "+5 Ranged Damage +10 % Damage +10 Range -5 % Attack Speed -5 % Crit Chance";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
