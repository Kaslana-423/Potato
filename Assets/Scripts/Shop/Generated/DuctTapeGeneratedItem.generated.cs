using System.Collections.Generic;

public sealed class DuctTapeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -2f, false),
        new ItemStatModifier("Engineering", 1f, false),
        new ItemStatModifier("Armor", 1f, false),
    };

    public override string Id => "item.duct_tape";
    public override string DisplayName => "Duct Tape";
    public override string Description => "+1 Armor +1 Engineering -2 Max HP";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
