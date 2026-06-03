using System.Collections.Generic;

public sealed class CampfireGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 2f, false),
        new ItemStatModifier("Elemental Damage", 2f, false),
        new ItemStatModifier("Speed", -2f, true),
    };

    public override string Id => "item.campfire";
    public override string DisplayName => "Campfire";
    public override string Description => "+2 Elemental Damage +2 HP Regeneration -2 % Speed";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
