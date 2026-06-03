using System.Collections.Generic;

public sealed class BatGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", 2f, true),
        new ItemStatModifier("Harvesting", -2f, false),
    };

    public override string Id => "item.bat";
    public override string DisplayName => "Bat";
    public override string Description => "+2 % Life Steal -2 Harvesting";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
