using System.Collections.Generic;

public sealed class GamblingTokenGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Armor", -1f, false),
        new ItemStatModifier("Dodge", 8f, true),
    };

    public override string Id => "item.gambling_token";
    public override string DisplayName => "Gambling Token";
    public override string Description => "+8 % Dodge -1 Armor";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
