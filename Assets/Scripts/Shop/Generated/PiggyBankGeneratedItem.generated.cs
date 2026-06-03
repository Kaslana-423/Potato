using System.Collections.Generic;

public sealed class PiggyBankGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.piggy_bank";
    public override string DisplayName => "Piggy Bank";
    public override string Description => "+20% of your materials at the start of waves (stops working at wave 20)";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
