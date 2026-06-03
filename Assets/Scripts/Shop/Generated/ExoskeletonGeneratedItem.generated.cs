using System.Collections.Generic;

public sealed class ExoskeletonGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -2f, false),
        new ItemStatModifier("Life Steal", -2f, true),
        new ItemStatModifier("Crit Chance", 5f, true),
        new ItemStatModifier("Engineering", 5f, false),
        new ItemStatModifier("Armor", 3f, false),
        new ItemStatModifier("Speed", 5f, true),
    };

    public override string Id => "item.exoskeleton";
    public override string DisplayName => "Exoskeleton";
    public override string Description => "+3 Armor +5 % Crit Chance +5 Engineering +5 % Speed -2 HP Regeneration -2 % Life Steal";
    public override int BasePrice => 90;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
