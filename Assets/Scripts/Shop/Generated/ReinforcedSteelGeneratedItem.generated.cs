using System.Collections.Generic;

public sealed class ReinforcedSteelGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", 2f, false),
        new ItemStatModifier("Engineering", 3f, false),
        new ItemStatModifier("Speed", -3f, true),
    };

    public override string Id => "item.reinforced_steel";
    public override string DisplayName => "Reinforced Steel";
    public override string Description => "+2 Ranged Damage +3 Engineering -3 % Speed";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
