using System.Collections.Generic;

public sealed class AlienBabyItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 15f)
    };

    public override string Id => "item.alien_baby";
    public override string DisplayName => "Alien Baby";
    public override string Description => "+15 Max HP and +10% enemy health.";
    public override string IconResourcePath => "IconImage/Items/alien-baby";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
