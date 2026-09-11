using System.Collections.Generic;

public sealed class AlienBabyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Speed", 10f, true),
    };

    public override string Id => "item.alien_baby";
    public override string DisplayName => "Alien-Baby";
    public override string Description => "+10% Speed ";
    public override string IconResourcePath => "IconImage/Items/alien-baby";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
