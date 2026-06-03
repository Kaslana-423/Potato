using System.Collections.Generic;

public sealed class GlassCannonGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 25f, true),
        new ItemStatModifier("Armor", -3f, false),
    };

    public override string Id => "item.glass_cannon";
    public override string DisplayName => "Glass Cannon";
    public override string Description => "+25 % Damage -3 Armor";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
