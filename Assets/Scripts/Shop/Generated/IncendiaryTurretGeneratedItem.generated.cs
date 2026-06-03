using System.Collections.Generic;

public sealed class IncendiaryTurretGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.incendiary_turret";
    public override string DisplayName => "Incendiary Turret";
    public override string Description => "Spawns a turret that shoots flames dealing 8x 5 ( +33% ) burning damage";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
