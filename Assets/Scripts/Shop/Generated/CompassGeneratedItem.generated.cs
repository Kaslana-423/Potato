using System.Collections.Generic;

public sealed class CompassGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Crit Chance", -3f, true),
        new ItemStatModifier("Engineering", 3f, false),
        new ItemStatModifier("Speed", 5f, true),
    };

    public override string Id => "item.compass";
    public override string DisplayName => "Compass";
    public override string Description => "+5 % Speed +3 Engineering -3 % Crit Chance";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
