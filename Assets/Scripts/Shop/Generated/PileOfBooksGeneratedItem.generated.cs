using System.Collections.Generic;

public sealed class PileOfBooksGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Crit Chance", 3f, true),
        new ItemStatModifier("Engineering", 3f, false),
    };

    public override string Id => "item.pile_of_books";
    public override string DisplayName => "Pile of Books";
    public override string Description => "Your structures can crit +3 % Crit Chance +3 Engineering";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
