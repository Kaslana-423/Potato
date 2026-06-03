using System.Collections.Generic;

public sealed class SadTomatoGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 8f, false),
    };

    public override string Id => "item.sad_tomato";
    public override string DisplayName => "Sad Tomato";
    public override string Description => "+8 HP Regeneration Start waves with -50% HP";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
