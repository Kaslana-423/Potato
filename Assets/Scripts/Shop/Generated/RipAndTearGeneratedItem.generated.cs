using System.Collections.Generic;

public sealed class RipAndTearGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Crit Chance", -5f, true),
    };

    public override string Id => "item.rip_and_tear";
    public override string DisplayName => "Rip and Tear";
    public override string Description => "Enemies have a 20% chance to explode for 10 ( +50% ) damage when they die -5 % Crit Chance";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 5;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
