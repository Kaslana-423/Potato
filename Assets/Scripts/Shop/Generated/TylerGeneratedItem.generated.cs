using System.Collections.Generic;

public sealed class TylerGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.tyler";
    public override string DisplayName => "Tyler";
    public override string Description => "Spawns a little guy that slowly shoots 10 piercing lightning projectiles around him for 10 ( +75% +75% ) damage each";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
