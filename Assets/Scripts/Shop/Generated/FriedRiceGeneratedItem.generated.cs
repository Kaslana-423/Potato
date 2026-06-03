using System.Collections.Generic;

public sealed class FriedRiceGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 1f, false),
    };

    public override string Id => "item.fried_rice";
    public override string DisplayName => "Fried Rice";
    public override string Description => "+1 HP Regeneration for every currently burning enemy";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
