using System.Collections.Generic;

public sealed class CyberballGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.cyberball";
    public override string DisplayName => "Cyberball";
    public override string Description => "25% chance to deal 1 ( 25% ) damage to a random enemy when an enemy dies";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
