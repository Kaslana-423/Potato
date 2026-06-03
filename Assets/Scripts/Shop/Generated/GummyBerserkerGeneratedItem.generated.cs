using System.Collections.Generic;

public sealed class GummyBerserkerGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", 5f, true),
        new ItemStatModifier("Range", 25f, false),
        new ItemStatModifier("Armor", -1f, false),
    };

    public override string Id => "item.gummy_berserker";
    public override string DisplayName => "Gummy Berserker";
    public override string Description => "+5 % Attack Speed +25 Range -1 Armor";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
