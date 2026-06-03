using System.Collections.Generic;

public sealed class MetalPlateGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -3f, true),
        new ItemStatModifier("Armor", 2f, false),
    };

    public override string Id => "item.metal_plate";
    public override string DisplayName => "Metal Plate";
    public override string Description => "+2 Armor -3 % Damage";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
