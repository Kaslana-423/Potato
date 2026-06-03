using System.Collections.Generic;

public sealed class StrangeBookGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", -1f, false),
        new ItemStatModifier("Ranged Damage", -1f, false),
        new ItemStatModifier("Engineering", 1f, false),
    };

    public override string Id => "item.strange_book";
    public override string DisplayName => "Strange Book";
    public override string Description => "+1 Engineering for every permanent 1 Elemental Damage you have -1 Melee Damage -1 Ranged Damage";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
