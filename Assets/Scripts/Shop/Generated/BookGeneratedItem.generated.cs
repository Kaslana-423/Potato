using System.Collections.Generic;

public sealed class BookGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Engineering", 1f, false),
    };

    public override string Id => "item.book";
    public override string DisplayName => "Book";
    public override string Description => "+1 Engineering";
    public override int BasePrice => 8;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
