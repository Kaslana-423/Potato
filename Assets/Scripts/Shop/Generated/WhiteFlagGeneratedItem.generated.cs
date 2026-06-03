using System.Collections.Generic;

public sealed class WhiteFlagGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Harvesting", 5f, false),
    };

    public override string Id => "item.white_flag";
    public override string DisplayName => "White Flag";
    public override string Description => "+5 Harvesting -5% Enemies";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
