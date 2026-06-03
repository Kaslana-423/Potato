using System.Collections.Generic;

public sealed class GoldfishDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.goldfish_dlc";
    public override string DisplayName => "Goldfish (DLC)";
    public override string Description => "Items will be 1 tier higher after the next reroll";
    public override int BasePrice => 23;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
