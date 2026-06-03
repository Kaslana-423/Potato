using System.Collections.Generic;

public sealed class WolfHelmetGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Elemental Damage", 10f, false),
        new ItemStatModifier("Engineering", -5f, false),
        new ItemStatModifier("Luck", 20f, false),
    };

    public override string Id => "item.wolf_helmet";
    public override string DisplayName => "Wolf Helmet";
    public override string Description => "+10 Elemental Damage +20 Luck -5 Engineering";
    public override int BasePrice => 90;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
