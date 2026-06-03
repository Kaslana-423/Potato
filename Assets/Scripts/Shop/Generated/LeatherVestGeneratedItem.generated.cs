using System.Collections.Generic;

public sealed class LeatherVestGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -3f, false),
        new ItemStatModifier("Armor", 2f, false),
        new ItemStatModifier("Dodge", 6f, true),
    };

    public override string Id => "item.leather_vest";
    public override string DisplayName => "Leather Vest";
    public override string Description => "+2 Armor +6 % Dodge -3 Max HP";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
