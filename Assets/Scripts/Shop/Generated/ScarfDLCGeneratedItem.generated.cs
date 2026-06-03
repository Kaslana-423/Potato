using System.Collections.Generic;

public sealed class ScarfDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 4f, false),
        new ItemStatModifier("Melee Damage", 4f, false),
        new ItemStatModifier("Speed", 4f, true),
    };

    public override string Id => "item.scarf_dlc";
    public override string DisplayName => "Scarf (DLC)";
    public override string Description => "+4 HP Regeneration +4 Melee Damage +4 % Speed +25 % Enemy Speed during the next wave";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
