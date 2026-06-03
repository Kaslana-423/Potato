using System.Collections.Generic;

public sealed class LumberjackShirtGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.lumberjack_shirt";
    public override string DisplayName => "Lumberjack Shirt";
    public override string Description => "Trees die in one hit";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
