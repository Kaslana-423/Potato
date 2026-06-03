using System.Collections.Generic;

public sealed class UglyToothGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Speed", -3f, true),
    };

    public override string Id => "item.ugly_tooth";
    public override string DisplayName => "Ugly Tooth";
    public override string Description => "Hitting an enemy removes 5% of their speed. Max 20% -3 % Speed";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
