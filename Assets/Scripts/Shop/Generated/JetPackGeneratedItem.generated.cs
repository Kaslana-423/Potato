using System.Collections.Generic;

public sealed class JetPackGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -5f, false),
        new ItemStatModifier("Armor", -1f, false),
        new ItemStatModifier("Dodge", 10f, true),
        new ItemStatModifier("Speed", 15f, true),
    };

    public override string Id => "item.jet_pack";
    public override string DisplayName => "Jet Pack";
    public override string Description => "+15 % Speed +10 % Dodge -5 Max HP -1 Armor";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
