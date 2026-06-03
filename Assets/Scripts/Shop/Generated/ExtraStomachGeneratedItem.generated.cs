using System.Collections.Generic;

public sealed class ExtraStomachGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 1f, false),
    };

    public override string Id => "item.extra_stomach";
    public override string DisplayName => "Extra Stomach";
    public override string Description => "+1 Max HP when picking up a consumable while at maximum health (max +8 per wave)";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
