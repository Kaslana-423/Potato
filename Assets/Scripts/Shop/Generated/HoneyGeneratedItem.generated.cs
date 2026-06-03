using System.Collections.Generic;

public sealed class HoneyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", 3f, false),
        new ItemStatModifier("Explosion Damage", 10f, true),
        new ItemStatModifier("Dodge", -3f, true),
        new ItemStatModifier("Speed", -3f, true),
    };

    public override string Id => "item.honey";
    public override string DisplayName => "Honey";
    public override string Description => "+3 Ranged Damage +10 % Explosion Damage +5 % Explosion Size -3 % Speed -3 % Dodge";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
