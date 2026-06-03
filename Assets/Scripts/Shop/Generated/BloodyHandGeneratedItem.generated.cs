using System.Collections.Generic;

public sealed class BloodyHandGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Life Steal", 10f, true),
        new ItemStatModifier("Damage", 2f, true),
    };

    public override string Id => "item.bloody_hand";
    public override string DisplayName => "Bloody Hand";
    public override string Description => "+10 % Life Steal +2 % Damage for every 1 % Life Steal you have You take 1 damage per second (does not give invulnerability time)";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
