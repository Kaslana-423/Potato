using System.Collections.Generic;

public sealed class GiantBeltGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.giant_belt";
    public override string DisplayName => "Giant Belt";
    public override string Description => "Critical hits deal 10% of an enemy’s current health as bonus damage ( 1% for bosses and elites)";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
