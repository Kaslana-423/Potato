using System.Collections.Generic;

public sealed class DynamiteGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Explosion Damage", 15f, true),
    };

    public override string Id => "item.dynamite";
    public override string DisplayName => "Dynamite";
    public override string Description => "+15 % Explosion Damage";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
