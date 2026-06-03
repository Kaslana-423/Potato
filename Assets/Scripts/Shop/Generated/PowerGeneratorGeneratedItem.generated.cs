using System.Collections.Generic;

public sealed class PowerGeneratorGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -4f, true),
    };

    public override string Id => "item.power_generator";
    public override string DisplayName => "Power Generator";
    public override string Description => "+1 % Damage for every permanent 1 % Speed you have -5 % Damage";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
