using System.Collections.Generic;

public sealed class PeacefulBeeGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Melee Damage", -1f, false),
        new ItemStatModifier("Ranged Damage", -1f, false),
        new ItemStatModifier("Dodge", 4f, true),
        new ItemStatModifier("Harvesting", 4f, false),
    };

    public override string Id => "item.peaceful_bee";
    public override string DisplayName => "Peaceful Bee";
    public override string Description => "+4 % Dodge +4 Harvesting -1 Melee Damage -1 Ranged Damage";
    public override int BasePrice => 18;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
