using System.Collections.Generic;

public sealed class FruitBasketGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -3f, false),
    };

    public override string Id => "item.fruit_basket";
    public override string DisplayName => "Fruit Basket";
    public override string Description => "Enemies have a higher chance of dropping fruits -3 HP Regeneration";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 4;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
