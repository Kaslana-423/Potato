using System.Collections.Generic;

public sealed class HourglassGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.hourglass";
    public override string DisplayName => "Hourglass";
    public override string Description => "Turns back time, decreasing the current wave count by 1 Start the next wave with 1 HP";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
