using System.Collections.Generic;

public sealed class TerrifiedOnionGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Speed", 4f, true),
        new ItemStatModifier("Luck", -5f, false),
    };

    public override string Id => "item.terrified_onion";
    public override string DisplayName => "Terrified Onion";
    public override string Description => "+4 % Speed -5 Luck";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
