using System.Collections.Generic;

public sealed class AlienBabyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 15f, false),
    };

    public override string Id => "item.alien_baby";
    public override string DisplayName => "Alien Baby";
    public override string Description => "+15 Max HP +10 % Enemy health";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
