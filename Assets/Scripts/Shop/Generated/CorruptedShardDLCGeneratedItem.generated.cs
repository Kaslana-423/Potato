using System.Collections.Generic;

public sealed class CorruptedShardDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 3f, true),
        new ItemStatModifier("Curse", 1f, false),
    };

    public override string Id => "item.corrupted_shard_dlc";
    public override string DisplayName => "Corrupted Shard (DLC)";
    public override string Description => "+3 % Damage +1 Curse";
    public override int BasePrice => 12;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
