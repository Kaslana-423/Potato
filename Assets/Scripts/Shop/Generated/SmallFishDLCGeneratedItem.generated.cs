using System.Collections.Generic;

public sealed class SmallFishDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 10f, true),
        new ItemStatModifier("Attack Speed", -3f, true),
    };

    public override string Id => "item.small_fish_dlc";
    public override string DisplayName => "Small Fish (DLC)";
    public override string Description => "+10% damage against targets above 75% health -3 % Attack Speed";
    public override int BasePrice => 18;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
