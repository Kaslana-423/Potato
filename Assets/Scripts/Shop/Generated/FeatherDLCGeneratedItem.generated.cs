using System.Collections.Generic;

public sealed class FeatherDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", 1f, false),
        new ItemStatModifier("Dodge", 3f, true),
        new ItemStatModifier("XP Gain", -3f, true),
    };

    public override string Id => "item.feather_dlc";
    public override string DisplayName => "Feather (DLC)";
    public override string Description => "+1 Ranged Damage +3 % Dodge -3 % XP Gain";
    public override int BasePrice => 18;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
