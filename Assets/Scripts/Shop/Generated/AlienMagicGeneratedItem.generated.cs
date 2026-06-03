using System.Collections.Generic;

public sealed class AlienMagicGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 8f, false),
        new ItemStatModifier("HP Regeneration", 3f, false),
        new ItemStatModifier("Luck", -8f, false),
    };

    public override string Id => "item.alien_magic";
    public override string DisplayName => "Alien Magic";
    public override string Description => "+8 Max HP +3 HP Regeneration -8 Luck";
    public override int BasePrice => 85;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
