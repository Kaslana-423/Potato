using System.Collections.Generic;

public sealed class SnowballGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Elemental Damage", 1f, false),
    };

    public override string Id => "item.snowball";
    public override string DisplayName => "Snowball";
    public override string Description => "+1 Elemental Damage every time you get an item that increases Elemental Damage";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
