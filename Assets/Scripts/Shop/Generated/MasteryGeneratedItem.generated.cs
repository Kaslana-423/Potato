using System.Collections.Generic;

public sealed class MasteryGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 6f, false),
        new ItemStatModifier("Ranged Damage", -3f, false),
    };

    public override string Id => "item.mastery";
    public override string DisplayName => "Mastery";
    public override string Description => "+6 Melee Damage -3 Ranged Damage";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
