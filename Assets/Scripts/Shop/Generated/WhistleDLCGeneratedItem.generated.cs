using System.Collections.Generic;

public sealed class WhistleDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.whistle_dlc";
    public override string DisplayName => "Whistle (DLC)";
    public override string Description => "+50% chance for loot aliens to appear +20% movement speed for loot aliens";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
