using System.Collections.Generic;

public sealed class SunkenBellDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.sunken_bell_dlc";
    public override string DisplayName => "Sunken Bell (DLC)";
    public override string Description => "Once per wave, you explode for 100 ( +500% +500% +500% +500% ) damage when you fall below 40% health";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
