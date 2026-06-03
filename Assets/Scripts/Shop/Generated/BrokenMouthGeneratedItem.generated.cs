using System.Collections.Generic;

public sealed class BrokenMouthGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 5f, false),
        new ItemStatModifier("HP Regeneration", -1f, false),
    };

    public override string Id => "item.broken_mouth";
    public override string DisplayName => "Broken Mouth";
    public override string Description => "+5 Max HP -1 HP Regeneration";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
