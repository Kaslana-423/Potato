using System.Collections.Generic;

public sealed class CyclopsWormGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 12f, true),
        new ItemStatModifier("Range", -12f, false),
    };

    public override string Id => "item.cyclops_worm";
    public override string DisplayName => "Cyclops Worm";
    public override string Description => "+12 % Damage -12 Range";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
