using System.Collections.Generic;

public sealed class MouseGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", 5f, true),
        new ItemStatModifier("Harvesting", -5f, false),
        new ItemStatModifier("Enemies", 10f, true),
    };

    public override string Id => "item.mouse";
    public override string DisplayName => "Mouse";
    public override string Description => "+5 % Life Steal +10% Enemies -5 Harvesting";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 5;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
