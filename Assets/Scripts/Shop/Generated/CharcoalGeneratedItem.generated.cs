using System.Collections.Generic;

public sealed class CharcoalGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 2f, false),
        new ItemStatModifier("Elemental Damage", 1f, false),
        new ItemStatModifier("Harvesting", -2f, false),
    };

    public override string Id => "item.charcoal";
    public override string DisplayName => "Charcoal";
    public override string Description => "+1 Elemental Damage +2 Melee Damage -2 Harvesting";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
