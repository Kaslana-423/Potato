using System.Collections.Generic;

public sealed class BloodLeechGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 2f, false),
        new ItemStatModifier("Life Steal", 2f, true),
        new ItemStatModifier("Harvesting", -3f, false),
    };

    public override string Id => "item.blood_leech";
    public override string DisplayName => "Blood Leech";
    public override string Description => "+2 % Life Steal +2 HP Regeneration -3 Harvesting";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
