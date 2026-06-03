using System.Collections.Generic;

public sealed class InsanityGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -3f, true),
        new ItemStatModifier("Crit Chance", 6f, true),
    };

    public override string Id => "item.insanity";
    public override string DisplayName => "Insanity";
    public override string Description => "+6 % Crit Chance -3 % Damage";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
