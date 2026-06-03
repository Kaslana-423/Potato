using System.Collections.Generic;

public sealed class AdrenalineItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Dodge", 5f, true)
    };

    public override string Id => "item.adrenaline";
    public override string DisplayName => "Adrenaline";
    public override string Description => "50% chance to heal 5 HP when dodging an attack.";
    public override string IconResourcePath => "IconImage/Items/adrenaline";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
