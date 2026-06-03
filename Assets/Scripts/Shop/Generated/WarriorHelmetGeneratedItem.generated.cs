using System.Collections.Generic;

public sealed class WarriorHelmetGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 5f, false),
        new ItemStatModifier("Armor", 3f, false),
        new ItemStatModifier("Speed", -5f, true),
    };

    public override string Id => "item.warrior_helmet";
    public override string DisplayName => "Warrior Helmet";
    public override string Description => "+3 Armor +5 Max HP -5 % Speed";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
