using System.Collections.Generic;

public sealed class LandminesGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.landmines";
    public override string DisplayName => "Landmines";
    public override string Description => "A landmine spawns every 12 seconds dealing 10 ( +100% ) damage in an area";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
