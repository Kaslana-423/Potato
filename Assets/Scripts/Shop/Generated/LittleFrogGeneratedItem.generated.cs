using System.Collections.Generic;

public sealed class LittleFrogGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Dodge", -3f, true),
        new ItemStatModifier("Harvesting", 10f, false),
        new ItemStatModifier("Pickup Range", 20f, true),
    };

    public override string Id => "item.little_frog";
    public override string DisplayName => "Little Frog";
    public override string Description => "+20% pickup range +10 Harvesting -3 % Dodge";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
