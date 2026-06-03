using System.Collections.Generic;

public sealed class TreasureMapDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.treasure_map_dlc";
    public override string DisplayName => "Treasure Map (DLC)";
    public override string Description => "+20% chance of finding an extra item in a crate";
    public override int BasePrice => 35;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 5;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
