using System.Collections.Generic;

public sealed class RiposteGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 2f, false),
    };

    public override string Id => "item.riposte";
    public override string DisplayName => "Riposte";
    public override string Description => "+2 Melee Damage 100% chance to deal 1 ( 300% ) damage to an enemy when dodging their attack";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
