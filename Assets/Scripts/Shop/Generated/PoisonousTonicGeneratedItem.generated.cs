using System.Collections.Generic;

public sealed class PoisonousTonicGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", -2f, false),
        new ItemStatModifier("Attack Speed", 10f, true),
        new ItemStatModifier("Crit Chance", 5f, true),
        new ItemStatModifier("Range", 15f, false),
    };

    public override string Id => "item.poisonous_tonic";
    public override string DisplayName => "Poisonous Tonic";
    public override string Description => "+10 % Attack Speed +5 % Crit Chance +15 Range -2 HP Regeneration";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
