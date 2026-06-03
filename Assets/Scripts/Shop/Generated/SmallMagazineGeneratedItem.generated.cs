using System.Collections.Generic;

public sealed class SmallMagazineGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -6f, true),
        new ItemStatModifier("Ranged Damage", 2f, false),
        new ItemStatModifier("Attack Speed", 10f, true),
    };

    public override string Id => "item.small_magazine";
    public override string DisplayName => "Small Magazine";
    public override string Description => "+2 Ranged Damage +10 % Attack Speed -6 % Damage";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
