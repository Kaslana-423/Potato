using System.Collections.Generic;

public sealed class BarricadeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Armor", 8f, false),
        new ItemStatModifier("Speed", -5f, true),
        new ItemStatModifier("Knockback", 3f, false),
    };

    public override string Id => "item.barricade";
    public override string DisplayName => "Barricade";
    public override string Description => "+3 Knockback +8 Armor while standing still -5 % Speed";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
