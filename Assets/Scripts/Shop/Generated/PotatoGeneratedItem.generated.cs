using System.Collections.Generic;

public sealed class PotatoGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 3f, false),
        new ItemStatModifier("HP Regeneration", 2f, false),
        new ItemStatModifier("Life Steal", 1f, true),
        new ItemStatModifier("Damage", 5f, true),
        new ItemStatModifier("Attack Speed", 5f, true),
        new ItemStatModifier("Armor", 1f, false),
        new ItemStatModifier("Dodge", 3f, true),
        new ItemStatModifier("Speed", 3f, true),
        new ItemStatModifier("Luck", 5f, false),
    };

    public override string Id => "item.potato";
    public override string DisplayName => "Potato";
    public override string Description => "+3 Max HP +2 HP Regeneration +1 % Life Steal +5 % Damage +5 % Attack Speed +3 % Speed +3 % Dodge +1 Armor +5 Luck";
    public override int BasePrice => 95;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
