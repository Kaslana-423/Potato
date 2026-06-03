using System.Collections.Generic;

public sealed class WeirdFoodGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Dodge", -2f, true),
    };

    public override string Id => "item.weird_food";
    public override string DisplayName => "Weird Food";
    public override string Description => "+2 HP recovered from consumables -2 % Dodge";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
