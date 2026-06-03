using System.Collections.Generic;

public sealed class CauldronDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -2f, false),
        new ItemStatModifier("Damage", 20f, true),
        new ItemStatModifier("Pickup Range", 50f, true),
    };

    public override string Id => "item.cauldron_dlc";
    public override string DisplayName => "Cauldron (DLC)";
    public override string Description => "+50% pickup range +20 % Damage for 2 seconds after picking up a consumable -2 HP Regeneration";
    public override int BasePrice => 35;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
