using System.Collections.Generic;

public sealed class AlienEyesGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.alien_eyes";
    public override string DisplayName => "Alien Eyes";
    public override string Description => "Shoots 6 alien eyes around you every 3 seconds dealing 6 ( +50% ) damage each";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
