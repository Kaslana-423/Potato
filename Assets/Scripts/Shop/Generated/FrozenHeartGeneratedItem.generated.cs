using System.Collections.Generic;

public sealed class FrozenHeartGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Elemental Damage", 8f, false),
        new ItemStatModifier("Crit Chance", 5f, true),
    };

    public override string Id => "item.frozen_heart";
    public override string DisplayName => "Frozen Heart";
    public override string Description => "+8 Elemental Damage +5 % Crit Chance Weapon damage additionally scales with 10% Elemental Damage Burning activates 100% slower";
    public override int BasePrice => 90;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
