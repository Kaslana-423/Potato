using System.Collections.Generic;

public sealed class ScaredSausageGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.scared_sausage";
    public override string DisplayName => "Scared Sausage";
    public override string Description => "Attacks have a 25% chance to deal 3x 1 ( +100% ) burning damage";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 4;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
