using System.Collections.Generic;

public sealed class BlackBeltGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 3f, false),
        new ItemStatModifier("Luck", -8f, false),
        new ItemStatModifier("Knockback", 3f, false),
        new ItemStatModifier("XP Gain", 25f, true),
    };

    public override string Id => "item.black_belt";
    public override string DisplayName => "Black Belt";
    public override string Description => "+25 % XP Gain +3 Melee Damage +3 Knockback -8 Luck";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
