using System.Collections.Generic;

public sealed class EyesSurgeryGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Elemental Damage", 1f, false),
        new ItemStatModifier("Range", -10f, false),
        new ItemStatModifier("Burning Speed", 20f, true),
    };

    public override string Id => "item.eyes_surgery";
    public override string DisplayName => "Eyes Surgery";
    public override string Description => "Burning activates 20% faster +1 Elemental Damage -10 Range";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 2;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
