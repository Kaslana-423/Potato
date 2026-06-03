using System.Collections.Generic;

public sealed class BandanaGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -10f, true),
    };

    public override string Id => "item.bandana";
    public override string DisplayName => "Bandana";
    public override string Description => "Projectiles pierce through 1 additional target -10 % Damage";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
