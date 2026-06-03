using System.Collections.Generic;

public sealed class LaserTurretGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.laser_turret";
    public override string DisplayName => "Laser Turret";
    public override string Description => "Spawns a turret that shoots piercing bullets dealing 20 ( +125% ) damage";
    public override int BasePrice => 65;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
