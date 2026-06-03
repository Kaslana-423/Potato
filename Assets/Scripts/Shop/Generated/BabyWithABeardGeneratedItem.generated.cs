using System.Collections.Generic;

public sealed class BabyWithABeardGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Range", -50f, false),
    };

    public override string Id => "item.baby_with_a_beard";
    public override string DisplayName => "Baby with a Beard";
    public override string Description => "One bullet dealing 1 ( +100% ) damage is fired from an enemy corpse when they die -50 Range";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
