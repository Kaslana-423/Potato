using System.Collections.Generic;

public sealed class MushroomGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 3f, false),
        new ItemStatModifier("Luck", -2f, false),
    };

    public override string Id => "item.mushroom";
    public override string DisplayName => "Mushroom";
    public override string Description => "+3 HP Regeneration -2 Luck";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
