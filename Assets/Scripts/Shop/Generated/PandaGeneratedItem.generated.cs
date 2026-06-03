using System.Collections.Generic;

public sealed class PandaGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 12f, false),
        new ItemStatModifier("Damage", -5f, true),
        new ItemStatModifier("Luck", 25f, false),
    };

    public override string Id => "item.panda";
    public override string DisplayName => "Panda";
    public override string Description => "+12 Max HP +25 Luck -5 % Damage";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
