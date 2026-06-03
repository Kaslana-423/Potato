using System.Collections.Generic;

public sealed class AxolotlDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.axolotl_dlc";
    public override string DisplayName => "Axolotl (DLC)";
    public override string Description => "Your highest ( Max HP ) and lowest ( Max HP ) positive primary stats are swapped when you get this item";
    public override int BasePrice => 130;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
