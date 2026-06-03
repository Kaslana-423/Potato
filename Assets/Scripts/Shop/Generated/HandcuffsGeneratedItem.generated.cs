using System.Collections.Generic;

public sealed class HandcuffsGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 8f, false),
        new ItemStatModifier("Ranged Damage", 8f, false),
        new ItemStatModifier("Elemental Damage", 8f, false),
    };

    public override string Id => "item.handcuffs";
    public override string DisplayName => "Handcuffs";
    public override string Description => "+8 Melee Damage +8 Ranged Damage +8 Elemental Damage Your Max HP is capped at its current value";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
