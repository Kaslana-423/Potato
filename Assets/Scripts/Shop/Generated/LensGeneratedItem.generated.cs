using System.Collections.Generic;

public sealed class LensGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Ranged Damage", 1f, false),
        new ItemStatModifier("Range", -5f, false),
    };

    public override string Id => "item.lens";
    public override string DisplayName => "Lens";
    public override string Description => "+1 Ranged Damage -5 Range";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
