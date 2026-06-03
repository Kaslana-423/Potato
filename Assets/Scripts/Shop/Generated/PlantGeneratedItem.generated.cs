using System.Collections.Generic;

public sealed class PlantGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 3f, false),
        new ItemStatModifier("Life Steal", -1f, true),
    };

    public override string Id => "item.plant";
    public override string DisplayName => "Plant";
    public override string Description => "+3 HP Regeneration -1 % Life Steal";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
