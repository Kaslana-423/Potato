using System.Collections.Generic;

public sealed class PenguinDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 2f, false),
    };

    public override string Id => "item.penguin_dlc";
    public override string DisplayName => "Penguin (DLC)";
    public override string Description => "+1 HP Regeneration +1 HP Regeneration until the end of the wave when picking up a consumable while at maximum health";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
