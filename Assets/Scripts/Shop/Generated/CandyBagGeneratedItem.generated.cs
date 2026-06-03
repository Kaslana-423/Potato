using System.Collections.Generic;

public sealed class CandyBagGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.candy_bag";
    public override string DisplayName => "Candy Bag";
    public override string Description => "Each wave grants 8 points randomly split between your primary stats Each wave, 10% chance to spawn an additional elite";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
