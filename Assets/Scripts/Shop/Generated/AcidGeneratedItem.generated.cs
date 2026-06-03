using System.Collections.Generic;

public sealed class AcidGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 8f, false),
        new ItemStatModifier("Dodge", -2f, true),
        new ItemStatModifier("Knockback", -2f, false),
    };

    public override string Id => "item.acid";
    public override string DisplayName => "Acid";
    public override string Description => "+8 Max HP -2 % Dodge -2 Knockback";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
