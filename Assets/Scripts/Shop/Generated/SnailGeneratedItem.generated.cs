using System.Collections.Generic;

public sealed class SnailGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Speed", -3f, true),
    };

    public override string Id => "item.snail";
    public override string DisplayName => "Snail";
    public override string Description => "-8 % Enemy Speed -3 % Speed";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
