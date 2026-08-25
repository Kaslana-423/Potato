using System.Collections.Generic;

public sealed class ClockworkWaspGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Speed", 5f, true),
        new ItemStatModifier("Structure Attack Speed", 10f, true),
    };

    public override string Id => "item.clockwork_wasp";
    public override string DisplayName => "Clockwork Wasp";
    public override string Description => "+10 % Structure attack speed +5 % Speed";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
