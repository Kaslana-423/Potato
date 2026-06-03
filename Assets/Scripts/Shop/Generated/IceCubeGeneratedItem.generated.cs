using System.Collections.Generic;

public sealed class IceCubeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.ice_cube";
    public override string DisplayName => "Ice Cube";
    public override string Description => "Enemies take 10% more damage for 3 seconds when first hit by Elemental Damage";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
