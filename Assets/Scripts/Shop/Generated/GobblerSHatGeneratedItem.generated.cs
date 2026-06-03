using System.Collections.Generic;

public sealed class GobblerSHatGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Dodge", -10f, true),
        new ItemStatModifier("Speed", -15f, true),
    };

    public override string Id => "item.gobbler_s_hat";
    public override string DisplayName => "Gobbler's Hat";
    public override string Description => "+70% materials dropped -15 % Speed -10 % Dodge";
    public override int BasePrice => 130;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
