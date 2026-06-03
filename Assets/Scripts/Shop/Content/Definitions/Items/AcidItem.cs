using System.Collections.Generic;

public sealed class AcidItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 8f),
        new ItemStatModifier("Dodge", -2f, true),
        new ItemStatModifier("Knockback", -2f)
    };

    public override string Id => "item.acid";
    public override string DisplayName => "Acid";
    public override string Description => "+8 Max HP, -2% Dodge and -2 Knockback.";
    public override string IconResourcePath => "IconImage/Items/acid";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
