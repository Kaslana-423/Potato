using System.Collections.Generic;

public sealed class CloverGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", -2f, true),
        new ItemStatModifier("Dodge", 6f, true),
        new ItemStatModifier("Luck", 20f, false),
    };

    public override string Id => "item.clover";
    public override string DisplayName => "Clover";
    public override string Description => "+20 Luck +6 % Dodge -2 % Life Steal";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
