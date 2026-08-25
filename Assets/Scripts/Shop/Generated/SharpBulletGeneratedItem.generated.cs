using System.Collections.Generic;

public sealed class SharpBulletGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -5f, true),
        new ItemStatModifier("Piercing Damage", -20f, true),
        new ItemStatModifier("Knockback", -3f, false),
        new ItemStatModifier("Piercing", 1f, false),
    };

    public override string Id => "item.sharp_bullet";
    public override string DisplayName => "Sharp Bullet";
    public override string Description => "Projectiles pierce through 1 additional target -20% Piercing Damage -5 % Damage -3 Knockback";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
