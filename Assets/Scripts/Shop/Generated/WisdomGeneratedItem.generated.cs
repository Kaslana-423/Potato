using System.Collections.Generic;

public sealed class WisdomGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -10f, true),
    };

    public override string Id => "item.wisdom";
    public override string DisplayName => "Wisdom";
    public override string Description => "+5 % Damage every 5 seconds until the end of the wave -15 % Damage";
    public override int BasePrice => 85;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
