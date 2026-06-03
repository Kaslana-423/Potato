using System.Collections.Generic;

public sealed class AnvilGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Armor", 2f, false),
    };

    public override string Id => "item.anvil";
    public override string DisplayName => "Anvil";
    public override string Description => "A random weapon is upgraded when entering a shop. If you have no weapon to upgrade, you gain +2 Armor instead.";
    public override int BasePrice => 120;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
