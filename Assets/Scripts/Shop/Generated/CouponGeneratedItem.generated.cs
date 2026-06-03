using System.Collections.Generic;

public sealed class CouponGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Items Price", -5f, true),
    };

    public override string Id => "item.coupon";
    public override string DisplayName => "Coupon";
    public override string Description => "-5 % Items Price";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 5;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
