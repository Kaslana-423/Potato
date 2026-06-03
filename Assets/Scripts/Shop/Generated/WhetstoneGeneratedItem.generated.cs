using System.Collections.Generic;

public sealed class WhetstoneGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", 4f, true),
        new ItemStatModifier("Knockback", -3f, false),
    };

    public override string Id => "item.whetstone";
    public override string DisplayName => "Whetstone";
    public override string Description => "+4 % Life Steal -3 Knockback";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
