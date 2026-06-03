using System.Collections.Generic;

public sealed class OctopusGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 12f, false),
        new ItemStatModifier("HP Regeneration", 5f, false),
        new ItemStatModifier("Life Steal", 3f, true),
        new ItemStatModifier("Crit Chance", -8f, true),
    };

    public override string Id => "item.octopus";
    public override string DisplayName => "Octopus";
    public override string Description => "+12 Max HP +5 HP Regeneration +3 % Life Steal -8 % Crit Chance";
    public override int BasePrice => 105;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
