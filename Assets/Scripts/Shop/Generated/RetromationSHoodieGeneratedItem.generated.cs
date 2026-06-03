using System.Collections.Generic;

public sealed class RetromationSHoodieGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", 2f, true),
        new ItemStatModifier("Range", -80f, false),
    };

    public override string Id => "item.retromation_s_hoodie";
    public override string DisplayName => "Retromation's Hoodie";
    public override string Description => "+2 % Attack Speed for every 1 % Dodge you have -80 Range";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
