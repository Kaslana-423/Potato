using System.Collections.Generic;

public sealed class BeanTeacherGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", -2f, true),
        new ItemStatModifier("XP Gain", 50f, true),
    };

    public override string Id => "item.bean_teacher";
    public override string DisplayName => "Bean Teacher";
    public override string Description => "+50 % XP Gain -2 % Life Steal";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
