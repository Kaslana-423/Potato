using System.Collections.Generic;

public sealed class WillOWispGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Elemental Damage", 1f, false),
        new ItemStatModifier("Attack Speed", -7f, true),
    };

    public override string Id => "item.will_o_wisp";
    public override string DisplayName => "Will-o'-Wisp";
    public override string Description => "+1 Elemental Damage for every 30 burning enemies you kill during a wave (max +4 per wave) -7 % Attack Speed";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
