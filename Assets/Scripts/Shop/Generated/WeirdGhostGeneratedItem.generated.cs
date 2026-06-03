using System.Collections.Generic;

public sealed class WeirdGhostGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 3f, false),
    };

    public override string Id => "item.weird_ghost";
    public override string DisplayName => "Weird Ghost";
    public override string Description => "+3 Max HP Start the next wave with 1 HP";
    public override int BasePrice => 12;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
