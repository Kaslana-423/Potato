using System.Collections.Generic;

public sealed class GrindSMagicalLeafGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 3f, false),
        new ItemStatModifier("HP Regeneration", 1f, false),
        new ItemStatModifier("Life Steal", 1f, true),
    };

    public override string Id => "item.grind_s_magical_leaf";
    public override string DisplayName => "Grind's Magical Leaf";
    public override string Description => "+3 Max HP at the end of a wave +1 HP Regeneration at the end of a wave +1 % Life Steal at the end of a wave";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
