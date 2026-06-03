using System.Collections.Generic;

public sealed class ScarGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Range", -8f, false),
        new ItemStatModifier("XP Gain", 20f, true),
    };

    public override string Id => "item.scar";
    public override string DisplayName => "Scar";
    public override string Description => "+20 % XP Gain -8 Range";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
