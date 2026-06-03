using System.Collections.Generic;

public sealed class CoralDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 10f, false),
        new ItemStatModifier("Range", -10f, false),
    };

    public override string Id => "item.coral_dlc";
    public override string DisplayName => "Coral (DLC)";
    public override string Description => "+10 HP Regeneration while standing still -10 Range";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
