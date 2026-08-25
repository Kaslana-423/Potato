using System.Collections.Generic;

public sealed class SnakeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -1f, false),
        new ItemStatModifier("Burning Spread", 1f, false),
    };

    public override string Id => "item.snake";
    public override string DisplayName => "Snake";
    public override string Description => "Burning spreads to an additional nearby enemy -1 Max HP";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
