using System.Collections.Generic;

public sealed class MutationGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", 1f, false),
        new ItemStatModifier("Elemental Damage", 1f, false),
        new ItemStatModifier("Knockback", -3f, false),
    };

    public override string Id => "item.mutation";
    public override string DisplayName => "Mutation";
    public override string Description => "+1 Ranged Damage +1 Elemental Damage -3 Knockback";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
