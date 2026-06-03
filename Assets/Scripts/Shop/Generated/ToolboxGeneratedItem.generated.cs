using System.Collections.Generic;

public sealed class ToolboxGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", -8f, true),
        new ItemStatModifier("Engineering", 6f, false),
    };

    public override string Id => "item.toolbox";
    public override string DisplayName => "Toolbox";
    public override string Description => "+6 Engineering -8 % Attack Speed";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
