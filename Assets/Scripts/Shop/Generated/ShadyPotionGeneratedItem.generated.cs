using System.Collections.Generic;

public sealed class ShadyPotionGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -2f, false),
        new ItemStatModifier("Luck", 20f, false),
    };

    public override string Id => "item.shady_potion";
    public override string DisplayName => "Shady Potion";
    public override string Description => "+20 Luck -2 HP Regeneration";
    public override int BasePrice => 48;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
