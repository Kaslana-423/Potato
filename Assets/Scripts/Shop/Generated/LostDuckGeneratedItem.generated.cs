using System.Collections.Generic;

public sealed class LostDuckGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Elemental Damage", -1f, false),
        new ItemStatModifier("Luck", 8f, false),
    };

    public override string Id => "item.lost_duck";
    public override string DisplayName => "Lost Duck";
    public override string Description => "+8 Luck -1 Elemental Damage";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
