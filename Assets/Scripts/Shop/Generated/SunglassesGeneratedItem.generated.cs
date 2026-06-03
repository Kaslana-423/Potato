using System.Collections.Generic;

public sealed class SunglassesGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Crit Chance", 10f, true),
        new ItemStatModifier("Armor", -1f, false),
    };

    public override string Id => "item.sunglasses";
    public override string DisplayName => "Sunglasses";
    public override string Description => "+10 % Crit Chance -1 Armor";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
