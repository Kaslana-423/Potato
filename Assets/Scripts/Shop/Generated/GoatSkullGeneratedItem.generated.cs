using System.Collections.Generic;

public sealed class GoatSkullGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", 3f, false),
        new ItemStatModifier("Crit Chance", -2f, true),
    };

    public override string Id => "item.goat_skull";
    public override string DisplayName => "Goat Skull";
    public override string Description => "+3 Melee Damage -2 % Crit Chance";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
