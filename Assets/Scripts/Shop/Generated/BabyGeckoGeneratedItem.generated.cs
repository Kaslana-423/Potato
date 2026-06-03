using System.Collections.Generic;

public sealed class BabyGeckoGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Range", 10f, false),
    };

    public override string Id => "item.baby_gecko";
    public override string DisplayName => "Baby Gecko";
    public override string Description => "+10 Range +25% chance to instantly attract a material when it’s dropped";
    public override int BasePrice => 18;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 4;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
