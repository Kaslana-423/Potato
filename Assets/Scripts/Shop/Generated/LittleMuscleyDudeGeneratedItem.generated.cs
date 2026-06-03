using System.Collections.Generic;

public sealed class LittleMuscleyDudeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 5f, false),
        new ItemStatModifier("Melee Damage", 3f, false),
        new ItemStatModifier("Range", -15f, false),
    };

    public override string Id => "item.little_muscley_dude";
    public override string DisplayName => "Little Muscley Dude";
    public override string Description => "+3 Melee Damage +5 Max HP -15 Range";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
