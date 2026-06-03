using System.Collections.Generic;

public sealed class WanderingBotGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.wandering_bot";
    public override string DisplayName => "Wandering Bot";
    public override string Description => "Spawns a little bot that slows down nearby enemies";
    public override int BasePrice => 60;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
