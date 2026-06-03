using System.Collections.Generic;

public sealed class FishHookDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Curse", 1f, false),
    };

    public override string Id => "item.fish_hook_dlc";
    public override string DisplayName => "Fish Hook (DLC)";
    public override string Description => "Locked items and weapons have a 20% chance to become cursed when leaving the shop +1 Curse";
    public override int BasePrice => 35;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
