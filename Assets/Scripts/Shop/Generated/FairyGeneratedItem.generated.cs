using System.Collections.Generic;

public sealed class FairyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -2f, false),
    };

    public override string Id => "item.fairy";
    public override string DisplayName => "Fairy";
    public override string Description => "+1 HP Regeneration for every different Tier I item you have -3 HP Regeneration for every different Tier IV item you have";
    public override int BasePrice => 85;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
