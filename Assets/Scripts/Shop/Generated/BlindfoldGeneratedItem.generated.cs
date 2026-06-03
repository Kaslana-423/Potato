using System.Collections.Generic;

public sealed class BlindfoldGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Crit Chance", 5f, true),
        new ItemStatModifier("Range", -15f, false),
        new ItemStatModifier("Dodge", 5f, true),
    };

    public override string Id => "item.blindfold";
    public override string DisplayName => "Blindfold";
    public override string Description => "+5 % Crit Chance +5 % Dodge -15 Range";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
