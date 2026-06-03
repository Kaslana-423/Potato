using System.Collections.Generic;

public sealed class WingsGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Elemental Damage", -2f, false),
        new ItemStatModifier("Range", 30f, false),
        new ItemStatModifier("Speed", 10f, true),
    };

    public override string Id => "item.wings";
    public override string DisplayName => "Wings";
    public override string Description => "+10 % Speed +30 Range -2 Elemental Damage";
    public override int BasePrice => 85;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
