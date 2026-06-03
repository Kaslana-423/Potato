using System.Collections.Generic;

public sealed class PencilGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", -1f, true),
        new ItemStatModifier("Crit Chance", -1f, true),
        new ItemStatModifier("Engineering", 2f, false),
    };

    public override string Id => "item.pencil";
    public override string DisplayName => "Pencil";
    public override string Description => "+2 Engineering -1 % Attack Speed -1 % Crit Chance";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
