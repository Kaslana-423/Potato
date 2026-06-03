using System.Collections.Generic;

public sealed class EnergyBraceletGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", -2f, false),
        new ItemStatModifier("Elemental Damage", 2f, false),
        new ItemStatModifier("Crit Chance", 4f, true),
    };

    public override string Id => "item.energy_bracelet";
    public override string DisplayName => "Energy Bracelet";
    public override string Description => "+4 % Crit Chance +2 Elemental Damage -2 Ranged Damage";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
