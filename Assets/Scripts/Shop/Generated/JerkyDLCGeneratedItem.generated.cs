using System.Collections.Generic;

public sealed class JerkyDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.jerky_dlc";
    public override string DisplayName => "Jerky (DLC)";
    public override string Description => "+3 HP recovered from consumables Consumables heal you over 4 seconds instead of instantly";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
