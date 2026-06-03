using System.Collections.Generic;

public sealed class FertilizerGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", -1f, false),
        new ItemStatModifier("Harvesting", 8f, false),
    };

    public override string Id => "item.fertilizer";
    public override string DisplayName => "Fertilizer";
    public override string Description => "+8 Harvesting -1 Melee Damage";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
