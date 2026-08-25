using System.Collections.Generic;

public sealed class CuteMonkeyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", -1f, false),
        new ItemStatModifier("Materials Healing", 8f, true),
    };

    public override string Id => "item.cute_monkey";
    public override string DisplayName => "Cute Monkey";
    public override string Description => "+8% chance to heal 1 HP when picking up a material -1 Ranged Damage";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 13;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
