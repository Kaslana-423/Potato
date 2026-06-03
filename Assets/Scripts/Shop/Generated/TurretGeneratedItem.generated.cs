using System.Collections.Generic;

public sealed class TurretGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.turret";
    public override string DisplayName => "Turret";
    public override string Description => "Spawns a turret that shoots bullets dealing 10 ( +80% ) damage";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
