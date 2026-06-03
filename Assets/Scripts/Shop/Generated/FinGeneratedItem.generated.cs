using System.Collections.Generic;

public sealed class FinGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", 3f, true),
        new ItemStatModifier("Speed", 10f, true),
        new ItemStatModifier("Luck", -8f, false),
    };

    public override string Id => "item.fin";
    public override string DisplayName => "Fin";
    public override string Description => "+10 % Speed +3 % Life Steal -8 Luck";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
