using System.Collections.Generic;

public sealed class ShacklesGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 8f, false),
        new ItemStatModifier("Engineering", 8f, false),
        new ItemStatModifier("Range", 80f, false),
    };

    public override string Id => "item.shackles";
    public override string DisplayName => "Shackles";
    public override string Description => "+8 HP Regeneration +8 Engineering +80 Range Your Speed is capped at its current value";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
