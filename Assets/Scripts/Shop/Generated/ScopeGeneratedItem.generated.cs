using System.Collections.Generic;

public sealed class ScopeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", 2f, false),
        new ItemStatModifier("Attack Speed", -7f, true),
        new ItemStatModifier("Range", 25f, false),
    };

    public override string Id => "item.scope";
    public override string DisplayName => "Scope";
    public override string Description => "+2 Ranged Damage +25 Range -7 % Attack Speed";
    public override int BasePrice => 48;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
