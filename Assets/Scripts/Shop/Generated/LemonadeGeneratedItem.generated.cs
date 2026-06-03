using System.Collections.Generic;

public sealed class LemonadeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.lemonade";
    public override string DisplayName => "Lemonade";
    public override string Description => "+1 HP recovered from consumables";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
