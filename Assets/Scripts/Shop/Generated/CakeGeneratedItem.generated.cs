using System.Collections.Generic;

public sealed class CakeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 3f, false),
        new ItemStatModifier("Damage", -1f, true),
    };

    public override string Id => "item.cake";
    public override string DisplayName => "Cake";
    public override string Description => "+3 Max HP -1 % Damage";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
