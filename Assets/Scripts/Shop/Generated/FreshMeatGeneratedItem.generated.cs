using System.Collections.Generic;

public sealed class FreshMeatGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -1f, false),
        new ItemStatModifier("Life Steal", 2f, true),
    };

    public override string Id => "item.fresh_meat";
    public override string DisplayName => "Fresh Meat";
    public override string Description => "+2 % Life Steal -1 HP Regeneration";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
