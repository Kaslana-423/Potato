using System.Collections.Generic;

public sealed class HeadInjuryGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 6f, true),
        new ItemStatModifier("Range", -8f, false),
    };

    public override string Id => "item.head_injury";
    public override string DisplayName => "Head Injury";
    public override string Description => "+6 % Damage -8 Range";
    public override int BasePrice => 25;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
