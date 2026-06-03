using System.Collections.Generic;

public sealed class PaddingGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 4f, false),
    };

    public override string Id => "item.padding";
    public override string DisplayName => "Padding";
    public override string Description => "+3 Max HP +1 Max HP for every 80 Materials you have";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
