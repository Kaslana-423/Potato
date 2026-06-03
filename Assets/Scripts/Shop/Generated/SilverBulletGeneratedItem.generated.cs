using System.Collections.Generic;

public sealed class SilverBulletGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 25f, true),
    };

    public override string Id => "item.silver_bullet";
    public override string DisplayName => "Silver Bullet";
    public override string Description => "+25% damage against bosses and elites";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
