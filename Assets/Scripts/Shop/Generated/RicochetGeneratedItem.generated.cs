using System.Collections.Generic;

public sealed class RicochetGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -25f, true),
        new ItemStatModifier("Bounces", 1f, false),
    };

    public override string Id => "item.ricochet";
    public override string DisplayName => "Ricochet";
    public override string Description => "Your projectiles gain +1 bounce -25 % Damage";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
