using System.Collections.Generic;

public sealed class CrownGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.crown";
    public override string DisplayName => "Crown";
    public override string Description => "Harvesting increases by an additional 8% at the end of a wave";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
