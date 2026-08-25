using System.Collections.Generic;

public sealed class DangerousBunnyGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Free Rerolls", 1f, false),
    };

    public override string Id => "item.dangerous_bunny";
    public override string DisplayName => "Dangerous Bunny";
    public override string Description => "+1 free reroll in the shop";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 3;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
