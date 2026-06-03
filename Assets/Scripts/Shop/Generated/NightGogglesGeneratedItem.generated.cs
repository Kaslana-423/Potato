using System.Collections.Generic;

public sealed class NightGogglesGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -3f, false),
        new ItemStatModifier("Crit Chance", 15f, true),
        new ItemStatModifier("Range", 50f, false),
        new ItemStatModifier("Armor", -1f, false),
    };

    public override string Id => "item.night_goggles";
    public override string DisplayName => "Night Goggles";
    public override string Description => "+15 % Crit Chance +50 Range -3 Max HP -1 Armor";
    public override int BasePrice => 95;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
