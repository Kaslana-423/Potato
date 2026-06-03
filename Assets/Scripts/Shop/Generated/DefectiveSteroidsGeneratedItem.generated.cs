using System.Collections.Generic;

public sealed class DefectiveSteroidsGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 2f, false),
        new ItemStatModifier("Melee Damage", 2f, false),
        new ItemStatModifier("Attack Speed", -3f, true),
    };

    public override string Id => "item.defective_steroids";
    public override string DisplayName => "Defective Steroids";
    public override string Description => "+2 Max HP +2 Melee Damage -3 % Attack Speed";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
