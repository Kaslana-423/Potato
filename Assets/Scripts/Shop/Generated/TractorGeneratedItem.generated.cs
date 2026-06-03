using System.Collections.Generic;

public sealed class TractorGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -8f, true),
        new ItemStatModifier("Harvesting", 40f, false),
    };

    public override string Id => "item.tractor";
    public override string DisplayName => "Tractor";
    public override string Description => "+40 Harvesting -8 % Damage";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
