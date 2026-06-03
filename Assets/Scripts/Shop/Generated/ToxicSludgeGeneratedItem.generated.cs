using System.Collections.Generic;

public sealed class ToxicSludgeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Elemental Damage", 2f, false),
        new ItemStatModifier("Dodge", -2f, true),
    };

    public override string Id => "item.toxic_sludge";
    public override string DisplayName => "Toxic Sludge";
    public override string Description => "+2 Elemental Damage -2 % Dodge";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
