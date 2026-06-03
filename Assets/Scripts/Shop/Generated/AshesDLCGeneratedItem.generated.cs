using System.Collections.Generic;

public sealed class AshesDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 20f, true),
        new ItemStatModifier("Attack Speed", 20f, true),
        new ItemStatModifier("Range", 100f, false),
        new ItemStatModifier("Armor", -1f, false),
    };

    public override string Id => "item.ashes_dlc";
    public override string DisplayName => "Ashes (DLC)";
    public override string Description => "+20 % Damage +20 % Attack Speed +100 Range -1 Armor at the end of a wave";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
