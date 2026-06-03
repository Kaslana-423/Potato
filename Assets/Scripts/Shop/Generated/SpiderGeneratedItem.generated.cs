using System.Collections.Generic;

public sealed class SpiderGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 12f, true),
        new ItemStatModifier("Attack Speed", 6f, true),
        new ItemStatModifier("Dodge", -3f, true),
        new ItemStatModifier("Harvesting", -5f, false),
    };

    public override string Id => "item.spider";
    public override string DisplayName => "Spider";
    public override string Description => "+12 % Damage +6 % Attack Speed for every different weapon you have -3 % Dodge -5 Harvesting";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
