using System.Collections.Generic;

public sealed class HuntingTrophyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.hunting_trophy";
    public override string DisplayName => "Hunting Trophy";
    public override string Description => "33% chance to gain 1 material when killing an enemy with a critical hit";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
