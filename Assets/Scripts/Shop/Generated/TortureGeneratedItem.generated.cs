using System.Collections.Generic;

public sealed class TortureGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 15f, false),
    };

    public override string Id => "item.torture";
    public override string DisplayName => "Torture";
    public override string Description => "+15 Max HP Restore 5 HP per second. Cannot heal any other way.";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
