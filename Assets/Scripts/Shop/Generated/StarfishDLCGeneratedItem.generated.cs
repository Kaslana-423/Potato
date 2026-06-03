using System.Collections.Generic;

public sealed class StarfishDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Harvesting", 10f, false),
    };

    public override string Id => "item.starfish_dlc";
    public override string DisplayName => "Starfish (DLC)";
    public override string Description => "+20% materials dropped from enemies +10 Harvesting +15 % Enemy damage";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
