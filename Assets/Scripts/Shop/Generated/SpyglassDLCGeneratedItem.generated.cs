using System.Collections.Generic;

public sealed class SpyglassDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Range", 10f, false),
    };

    public override string Id => "item.spyglass_dlc";
    public override string DisplayName => "Spyglass (DLC)";
    public override string Description => "+10 Range -25 % Reroll Price";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 2;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
