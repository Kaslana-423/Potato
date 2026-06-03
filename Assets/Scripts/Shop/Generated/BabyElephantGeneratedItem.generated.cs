using System.Collections.Generic;

public sealed class BabyElephantGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.baby_elephant";
    public override string DisplayName => "Baby Elephant";
    public override string Description => "25% chance to deal 1 ( 25% ) damage to a random enemy when you pick up a material";
    public override int BasePrice => 22;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
