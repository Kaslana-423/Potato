using System.Collections.Generic;

public sealed class SpicySauceGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 3f, false),
    };

    public override string Id => "item.spicy_sauce";
    public override string DisplayName => "Spicy Sauce";
    public override string Description => "+3 Max HP Consumables have a 50% chance to explode for 10 ( +100% ) damage when picked up";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 2;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
