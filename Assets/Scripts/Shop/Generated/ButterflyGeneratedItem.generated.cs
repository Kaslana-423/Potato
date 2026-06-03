using System.Collections.Generic;

public sealed class ButterflyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", 2f, true),
        new ItemStatModifier("Elemental Damage", -1f, false),
    };

    public override string Id => "item.butterfly";
    public override string DisplayName => "Butterfly";
    public override string Description => "+2 % Life Steal -1 Elemental Damage";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
