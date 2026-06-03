using System.Collections.Generic;

public sealed class AlienWormGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 3f, false),
        new ItemStatModifier("HP Regeneration", 2f, false),
    };

    public override string Id => "item.alien_worm";
    public override string DisplayName => "Alien Worm";
    public override string Description => "+3 Max HP +2 HP Regeneration -1 HP recovered from consumables";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
