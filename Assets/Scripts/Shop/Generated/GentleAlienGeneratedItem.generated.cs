using System.Collections.Generic;

public sealed class GentleAlienGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 2f, false),
        new ItemStatModifier("Damage", 5f, true),
    };

    public override string Id => "item.gentle_alien";
    public override string DisplayName => "Gentle Alien";
    public override string Description => "+2 Max HP +5 % Damage +5% Enemies";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 10;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
