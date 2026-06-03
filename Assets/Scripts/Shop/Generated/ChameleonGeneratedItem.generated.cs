using System.Collections.Generic;

public sealed class ChameleonGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -4f, true),
        new ItemStatModifier("Dodge", 23f, true),
    };

    public override string Id => "item.chameleon";
    public override string DisplayName => "Chameleon";
    public override string Description => "+3 % Dodge +20 % Dodge while standing still -4 % Damage";
    public override int BasePrice => 70;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
