using System.Collections.Generic;

public sealed class LureGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 2f, false),
    };

    public override string Id => "item.lure";
    public override string DisplayName => "Lure";
    public override string Description => "+2 HP Regeneration 2 additional loot aliens appear during the next wave";
    public override int BasePrice => 34;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
