using System.Collections.Generic;

public sealed class GnomeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 10f, false),
        new ItemStatModifier("Elemental Damage", 10f, false),
        new ItemStatModifier("Range", -20f, false),
        new ItemStatModifier("Pickup Range", -20f, true),
    };

    public override string Id => "item.gnome";
    public override string DisplayName => "Gnome";
    public override string Description => "+10 Melee Damage +10 Elemental Damage -20 Range -20% pickup range";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
