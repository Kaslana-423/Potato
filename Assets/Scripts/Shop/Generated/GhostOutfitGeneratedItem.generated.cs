using System.Collections.Generic;

public sealed class GhostOutfitGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Armor", -3f, false),
        new ItemStatModifier("Dodge", 10f, true),
        new ItemStatModifier("Speed", -5f, true),
    };

    public override string Id => "item.ghost_outfit";
    public override string DisplayName => "Ghost Outfit";
    public override string Description => "Dodge is capped at 70% +10 % Dodge -5 % Speed -3 Armor";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
