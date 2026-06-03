using System.Collections.Generic;

public sealed class AlienTongueGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Knockback", 1f, false),
        new ItemStatModifier("Pickup Range", 30f, true),
    };

    public override string Id => "item.alien_tongue";
    public override string DisplayName => "Alien Tongue";
    public override string Description => "+30% pickup range +1 Knockback";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
