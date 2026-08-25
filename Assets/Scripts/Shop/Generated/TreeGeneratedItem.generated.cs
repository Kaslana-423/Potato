using System.Collections.Generic;

public sealed class TreeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Trees", 1f, false),
    };

    public override string Id => "item.tree";
    public override string DisplayName => "Tree";
    public override string Description => "More trees spawn";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
