using System.Collections.Generic;

public sealed class MetalDetectorGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -5f, true),
        new ItemStatModifier("Engineering", 2f, false),
        new ItemStatModifier("Luck", 6f, false),
    };

    public override string Id => "item.metal_detector";
    public override string DisplayName => "Metal Detector";
    public override string Description => "+5% chance to double the value of picked up materials +6 Luck +2 Engineering -5 % Damage";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 20;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
