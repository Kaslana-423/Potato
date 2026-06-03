using System.Collections.Generic;

public sealed class StatueGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Attack Speed", 40f, true),
        new ItemStatModifier("Speed", -10f, true),
    };

    public override string Id => "item.statue";
    public override string DisplayName => "Statue";
    public override string Description => "+40 % Attack Speed while standing still -10 % Speed";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
