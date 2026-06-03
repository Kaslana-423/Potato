using System.Collections.Generic;

public sealed class JellyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 1f, false),
    };

    public override string Id => "item.jelly";
    public override string DisplayName => "Jelly";
    public override string Description => "+1 Max HP for every different weapon you have";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
