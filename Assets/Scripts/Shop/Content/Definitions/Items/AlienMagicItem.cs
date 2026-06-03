using System.Collections.Generic;

public sealed class AlienMagicItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 8f),
        new ItemStatModifier("HP Regeneration", 3f),
        new ItemStatModifier("Luck", -8f)
    };

    public override string Id => "item.alien_magic";
    public override string DisplayName => "Alien Magic";
    public override string Description => "+8 Max HP, +3 HP Regeneration and -8 Luck.";
    public override string IconResourcePath => "IconImage/Items/alien-magic";
    public override int BasePrice => 85;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
