using System.Collections.Generic;

public sealed class TardigradeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.tardigrade";
    public override string DisplayName => "Tardigrade";
    public override string Description => "Nullifies the damage of one hit taken every wave";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
