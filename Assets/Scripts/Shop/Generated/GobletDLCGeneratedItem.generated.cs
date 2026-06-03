using System.Collections.Generic;

public sealed class GobletDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -3f, false),
    };

    public override string Id => "item.goblet_dlc";
    public override string DisplayName => "Goblet (DLC)";
    public override string Description => "+15% chance to heal 1 HP when killing an enemy -3 HP Regeneration";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
