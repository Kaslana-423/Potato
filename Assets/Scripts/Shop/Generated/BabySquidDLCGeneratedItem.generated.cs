using System.Collections.Generic;

public sealed class BabySquidDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 1f, false),
        new ItemStatModifier("Attack Speed", -3f, true),
    };

    public override string Id => "item.baby_squid_dlc";
    public override string DisplayName => "Baby Squid (DLC)";
    public override string Description => "+1 HP Regeneration when you level up -3 % Attack Speed";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
