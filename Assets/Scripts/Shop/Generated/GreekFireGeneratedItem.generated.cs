using System.Collections.Generic;

public sealed class GreekFireGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.greek_fire";
    public override string DisplayName => "Greek Fire";
    public override string Description => "Burning deals an additional 10% of current enemy HP as damage ( 1% for bosses and elites)";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
