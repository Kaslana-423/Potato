using System.Collections.Generic;

public sealed class BlackFlagDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Curse", 5f, false),
    };

    public override string Id => "item.black_flag_dlc";
    public override string DisplayName => "Black Flag (DLC)";
    public override string Description => "+1 material when you kill a cursed enemy +10% Enemies +10 % Enemy health +10 % Enemy damage +5 Curse";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
