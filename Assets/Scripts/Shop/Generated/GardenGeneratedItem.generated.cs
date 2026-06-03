using System.Collections.Generic;

public sealed class GardenGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.garden";
    public override string DisplayName => "Garden";
    public override string Description => "Spawns a garden that creates a fruit every 15 seconds";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
