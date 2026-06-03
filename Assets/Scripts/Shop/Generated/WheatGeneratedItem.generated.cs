using System.Collections.Generic;

public sealed class WheatGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 4f, false),
        new ItemStatModifier("Ranged Damage", 2f, false),
        new ItemStatModifier("Elemental Damage", -2f, false),
        new ItemStatModifier("Harvesting", 10f, false),
    };

    public override string Id => "item.wheat";
    public override string DisplayName => "Wheat";
    public override string Description => "+4 Melee Damage +2 Ranged Damage +10 Harvesting -2 Elemental Damage";
    public override int BasePrice => 85;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
