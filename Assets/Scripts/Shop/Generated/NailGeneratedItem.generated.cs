using System.Collections.Generic;

public sealed class NailGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", -2f, false),
        new ItemStatModifier("Engineering", 5f, false),
    };

    public override string Id => "item.nail";
    public override string DisplayName => "Nail";
    public override string Description => "+5 Engineering Weapon damage additionally scales with 20% Engineering -2 Ranged Damage";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
