using System.Collections.Generic;

public sealed class BarnacleDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Curse", 1f, false),
    };

    public override string Id => "item.barnacle_dlc";
    public override string DisplayName => "Barnacle (DLC)";
    public override string Description => "+35% stats gained from level upgrades +1 Curse when you level up";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
