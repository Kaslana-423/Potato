using System.Collections.Generic;

public sealed class DiplomaGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -3f, false),
        new ItemStatModifier("Engineering", 10f, false),
        new ItemStatModifier("XP Gain", 50f, true),
    };

    public override string Id => "item.diploma";
    public override string DisplayName => "Diploma";
    public override string Description => "+10 Engineering +50 % XP Gain -3 Max HP";
    public override int BasePrice => 90;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
