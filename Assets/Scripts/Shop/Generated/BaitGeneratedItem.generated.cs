using System.Collections.Generic;

public sealed class BaitGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 8f, true),
    };

    public override string Id => "item.bait";
    public override string DisplayName => "Bait";
    public override string Description => "+8 % Damage Special enemies appear at the beginning of the next wave";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
