using System.Collections.Generic;

public sealed class ClawTreeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -1f, false),
        new ItemStatModifier("Melee Damage", 1f, false),
        new ItemStatModifier("Crit Chance", 3f, true),
    };

    public override string Id => "item.claw_tree";
    public override string DisplayName => "Claw Tree";
    public override string Description => "+1 Melee Damage +3 % Crit Chance -1 Max HP";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
