using System.Collections.Generic;

public sealed class ExplosiveTurretGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.explosive_turret";
    public override string DisplayName => "Explosive Turret";
    public override string Description => "Spawns a turret that shoots explosive bullets dealing 25 ( +150% ) damage in an area";
    public override int BasePrice => 80;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
