using System.Collections.Generic;

public sealed class MirrorDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.mirror_dlc";
    public override string DisplayName => "Mirror (DLC)";
    public override string Description => "Duplicates the next item you get from the shop (item limits can't be exceeded)";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
