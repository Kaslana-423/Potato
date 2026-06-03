using System.Collections.Generic;

public sealed class PropellerHatGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -2f, true),
        new ItemStatModifier("Luck", 10f, false),
    };

    public override string Id => "item.propeller_hat";
    public override string DisplayName => "Propeller Hat";
    public override string Description => "+10 Luck -2 % Damage";
    public override int BasePrice => 28;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
