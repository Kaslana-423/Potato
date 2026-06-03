using System.Collections.Generic;

public sealed class CeleryTeaGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("XP Gain", 55f, true),
    };

    public override string Id => "item.celery_tea";
    public override string DisplayName => "Celery Tea";
    public override string Description => "+5 % XP Gain at the end of a wave +50 % XP Gain during the next wave +100 % Enemy health during the next wave";
    public override int BasePrice => 35;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
