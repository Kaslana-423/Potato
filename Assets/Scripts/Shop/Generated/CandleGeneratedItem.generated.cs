using System.Collections.Generic;

public sealed class CandleGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 1f, false),
        new ItemStatModifier("Damage", -5f, true),
        new ItemStatModifier("Elemental Damage", 4f, false),
        new ItemStatModifier("Enemies", -10f, true),
    };

    public override string Id => "item.candle";
    public override string DisplayName => "Candle";
    public override string Description => "+4 Elemental Damage +1 HP Regeneration -10% Enemies -5 % Damage";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
