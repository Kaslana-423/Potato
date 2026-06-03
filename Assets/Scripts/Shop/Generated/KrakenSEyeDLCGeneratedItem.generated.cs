using System.Collections.Generic;

public sealed class KrakenSEyeDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 10f, false),
        new ItemStatModifier("Curse", 10f, false),
    };

    public override string Id => "item.kraken_s_eye_dlc";
    public override string DisplayName => "Kraken's Eye (DLC)";
    public override string Description => "You have a 50% chance to explode for 10 ( +500% ) damage when you get hit +10 Max HP +10 Curse";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
