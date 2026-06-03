using System.Collections.Generic;

public sealed class LuckyCharmGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", -2f, false),
        new ItemStatModifier("Ranged Damage", -1f, false),
        new ItemStatModifier("Luck", 30f, false),
    };

    public override string Id => "item.lucky_charm";
    public override string DisplayName => "Lucky Charm";
    public override string Description => "+30 Luck -2 Melee Damage -1 Ranged Damage";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
