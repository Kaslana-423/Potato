using System.Collections.Generic;

public sealed class SifdSRelicGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Armor", 3f, false),
    };

    public override string Id => "item.sifd_s_relic";
    public override string DisplayName => "Sifd's Relic";
    public override string Description => "+3 Armor +100% chance to instantly attract a material when it’s dropped";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
