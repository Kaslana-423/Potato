using System.Collections.Generic;

public sealed class RecyclingMachineGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.recycling_machine";
    public override string DisplayName => "Recycling Machine";
    public override string Description => "Gain 35% more materials from recycling items";
    public override int BasePrice => 35;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
