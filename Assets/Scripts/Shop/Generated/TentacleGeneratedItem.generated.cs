using System.Collections.Generic;

public sealed class TentacleGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Crit Chance", 3f, true),
    };

    public override string Id => "item.tentacle";
    public override string DisplayName => "Tentacle";
    public override string Description => "+3 % Crit Chance +20% chance to heal 1 HP when killing an enemy with a critical hit";
    public override int BasePrice => 35;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 5;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
