using System.Collections.Generic;

public sealed class ImprovedToolsGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", 10f, true),
    };

    public override string Id => "item.improved_tools";
    public override string DisplayName => "Improved Tools";
    public override string Description => "+10 % Attack Speed Increases the attack speed of your structures by 0% ( 50% )";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
