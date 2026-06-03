using System.Collections.Generic;

public sealed class BoxingGloveGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 1f, false),
        new ItemStatModifier("Knockback", 3f, false),
    };

    public override string Id => "item.boxing_glove";
    public override string DisplayName => "Boxing Glove";
    public override string Description => "+1 Melee Damage +3 Knockback";
    public override int BasePrice => 18;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
