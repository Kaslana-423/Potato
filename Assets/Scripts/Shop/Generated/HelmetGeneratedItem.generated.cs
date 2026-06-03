using System.Collections.Generic;

public sealed class HelmetGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Armor", 1f, false),
        new ItemStatModifier("Speed", -2f, true),
    };

    public override string Id => "item.helmet";
    public override string DisplayName => "Helmet";
    public override string Description => "+1 Armor -2 % Speed";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
