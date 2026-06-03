using System.Collections.Generic;

public sealed class MissileGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 10f, true),
        new ItemStatModifier("Attack Speed", -4f, true),
    };

    public override string Id => "item.missile";
    public override string DisplayName => "Missile";
    public override string Description => "+10 % Damage -4 % Attack Speed";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
