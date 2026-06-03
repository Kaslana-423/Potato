using System.Collections.Generic;

public sealed class DecomposingFleshGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -1f, false),
        new ItemStatModifier("Life Steal", 1f, true),
    };

    public override string Id => "item.decomposing_flesh";
    public override string DisplayName => "Decomposing Flesh";
    public override string Description => "+1 % Life Steal when you level up -1 Max HP when you level up";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
