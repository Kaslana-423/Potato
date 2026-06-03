using System.Collections.Generic;

public sealed class LuckyCoinGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Armor", -2f, false),
        new ItemStatModifier("Luck", 2f, false),
    };

    public override string Id => "item.lucky_coin";
    public override string DisplayName => "Lucky Coin";
    public override string Description => "+2 Luck for every 1 % Crit Chance you have -2 Armor";
    public override int BasePrice => 105;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
