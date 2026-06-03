using System.Collections.Generic;

public sealed class BagGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Speed", -1f, true),
    };

    public override string Id => "item.bag";
    public override string DisplayName => "Bag";
    public override string Description => "+15 materials when you pick up a crate -1 % Speed";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
