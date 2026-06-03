using System.Collections.Generic;

public sealed class AlienTongueItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Pickup Range", 30f, true),
        new ItemStatModifier("Knockback", 1f)
    };

    public override string Id => "item.alien_tongue";
    public override string DisplayName => "Alien Tongue";
    public override string Description => "+30% Pickup Range and +1 Knockback.";
    public override string IconResourcePath => "IconImage/Items/alien-tongue";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
