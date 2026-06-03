using System.Collections.Generic;

public sealed class PlasticExplosiveGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.plastic_explosive";
    public override string DisplayName => "Plastic Explosive";
    public override string Description => "+25 % Explosion Size";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
