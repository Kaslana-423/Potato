using System.Collections.Generic;

public sealed class HedgehogGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -1f, false),
        new ItemStatModifier("Melee Damage", 2f, false),
        new ItemStatModifier("Ranged Damage", 1f, false),
    };

    public override string Id => "item.hedgehog";
    public override string DisplayName => "Hedgehog";
    public override string Description => "+2 Melee Damage +1 Ranged Damage -1 HP Regeneration";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
