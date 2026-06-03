using System.Collections.Generic;

public sealed class RitualGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", 2f, true),
        new ItemStatModifier("Damage", 6f, true),
        new ItemStatModifier("Engineering", -2f, false),
    };

    public override string Id => "item.ritual";
    public override string DisplayName => "Ritual";
    public override string Description => "+6 % Damage +2 % Life Steal -2 Engineering";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
