using System.Collections.Generic;

public sealed class CoffeeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -2f, true),
        new ItemStatModifier("Attack Speed", 10f, true),
    };

    public override string Id => "item.coffee";
    public override string DisplayName => "Coffee";
    public override string Description => "+10 % Attack Speed -2 % Damage";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
