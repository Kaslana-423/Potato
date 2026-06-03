using System.Collections.Generic;

public sealed class CrystalDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", 6f, true),
        new ItemStatModifier("Engineering", -2f, false),
    };

    public override string Id => "item.crystal_dlc";
    public override string DisplayName => "Crystal (DLC)";
    public override string Description => "+5 % Attack Speed +1 % Attack Speed every 1 second until the end of the wave Bonus is lost when taking damage -2 Engineering";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
