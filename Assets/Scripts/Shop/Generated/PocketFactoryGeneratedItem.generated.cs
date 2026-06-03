using System.Collections.Generic;

public sealed class PocketFactoryGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Engineering", 2f, false),
    };

    public override string Id => "item.pocket_factory";
    public override string DisplayName => "Pocket Factory";
    public override string Description => "+2 Engineering Killing a tree spawns a turret";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
