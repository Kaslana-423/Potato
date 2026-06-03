using System.Collections.Generic;

public sealed class CommunitySupportGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", 1f, true),
        new ItemStatModifier("Armor", -2f, false),
    };

    public override string Id => "item.community_support";
    public override string DisplayName => "Community Support";
    public override string Description => "+1 % Attack Speed for every current living enemy -2 Armor";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
