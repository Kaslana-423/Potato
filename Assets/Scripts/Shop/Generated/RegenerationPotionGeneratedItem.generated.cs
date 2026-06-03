using System.Collections.Generic;

public sealed class RegenerationPotionGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 3f, false),
    };

    public override string Id => "item.regeneration_potion";
    public override string DisplayName => "Regeneration Potion";
    public override string Description => "HP Regeneration is doubled when you have less than 50% health +3 HP Regeneration";
    public override int BasePrice => 90;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
