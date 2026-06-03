using System.Collections.Generic;

public sealed class MammothGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 5f, false),
        new ItemStatModifier("Damage", -8f, true),
        new ItemStatModifier("Melee Damage", 20f, false),
        new ItemStatModifier("Speed", -3f, true),
        new ItemStatModifier("Knockback", 5f, false),
    };

    public override string Id => "item.mammoth";
    public override string DisplayName => "Mammoth";
    public override string Description => "+20 Melee Damage +5 HP Regeneration +5 Knockback -8 % Damage -3 % Speed";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
