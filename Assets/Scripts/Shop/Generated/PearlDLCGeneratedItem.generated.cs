using System.Collections.Generic;

public sealed class PearlDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 1f, true),
    };

    public override string Id => "item.pearl_dlc";
    public override string DisplayName => "Pearl (DLC)";
    public override string Description => "+1 % Damage for every permanent 10 Luck you have +3% chance of finding an extra Pearl in a crate";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 20;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
