using System.Collections.Generic;

public sealed class BowlerHatGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", -5f, true),
        new ItemStatModifier("Crit Chance", -3f, true),
        new ItemStatModifier("Luck", 15f, false),
        new ItemStatModifier("Harvesting", 18f, false),
    };

    public override string Id => "item.bowler_hat";
    public override string DisplayName => "Bowler Hat";
    public override string Description => "+15 Luck +18 Harvesting -5 % Attack Speed -3 % Crit Chance";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
