using System.Collections.Generic;

public sealed class CoilGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 1f, true),
        new ItemStatModifier("Knockback", 5f, false),
    };

    public override string Id => "item.coil";
    public override string DisplayName => "Coil";
    public override string Description => "+5 Knockback +1 % Damage for every 1 Knockback you have";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
