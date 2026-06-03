using System.Collections.Generic;

public sealed class FocusGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 30f, true),
        new ItemStatModifier("Attack Speed", -3f, true),
    };

    public override string Id => "item.focus";
    public override string DisplayName => "Focus";
    public override string Description => "+30 % Damage -3 % Attack Speed for every different weapon you have";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
