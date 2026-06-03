using System.Collections.Generic;

public sealed class BannerGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", 10f, true),
        new ItemStatModifier("Range", 20f, false),
        new ItemStatModifier("Knockback", -5f, false),
    };

    public override string Id => "item.banner";
    public override string DisplayName => "Banner";
    public override string Description => "+20 Range +10 % Attack Speed -5 Knockback";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
