using System.Collections.Generic;

public sealed class PeacockGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("XP Gain", 125f, true),
    };

    public override string Id => "item.peacock";
    public override string DisplayName => "Peacock";
    public override string Description => "+25 % XP Gain +100 % XP Gain during the next wave +50 % Enemy damage during the next wave";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
