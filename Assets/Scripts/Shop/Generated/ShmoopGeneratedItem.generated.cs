using System.Collections.Generic;

public sealed class ShmoopGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 6f, false),
        new ItemStatModifier("HP Regeneration", 2f, false),
        new ItemStatModifier("Melee Damage", -2f, false),
        new ItemStatModifier("Ranged Damage", -1f, false),
    };

    public override string Id => "item.shmoop";
    public override string DisplayName => "Shmoop";
    public override string Description => "+6 Max HP +2 HP Regeneration -2 Melee Damage -1 Ranged Damage";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
