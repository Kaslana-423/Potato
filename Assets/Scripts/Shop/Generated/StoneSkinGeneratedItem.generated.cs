using System.Collections.Generic;

public sealed class StoneSkinGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 1f, false),
        new ItemStatModifier("Attack Speed", -6f, true),
    };

    public override string Id => "item.stone_skin";
    public override string DisplayName => "Stone Skin";
    public override string Description => "+1 Max HP for every permanent 1 Armor you have -6 % Attack Speed";
    public override int BasePrice => 85;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
