using System.Collections.Generic;

public sealed class CogGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -4f, true),
        new ItemStatModifier("Engineering", 4f, false),
        new ItemStatModifier("Knockback", 1f, false),
    };

    public override string Id => "item.cog";
    public override string DisplayName => "Cog";
    public override string Description => "+4 Engineering +1 Knockback -4 % Damage";
    public override int BasePrice => 35;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
