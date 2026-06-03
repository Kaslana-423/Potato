using System.Collections.Generic;

public sealed class MedicalTurretGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
    };

    public override string Id => "item.medical_turret";
    public override string DisplayName => "Medical Turret";
    public override string Description => "Spawns a medical turret that shoots bullets healing 3 ( +5% ) HP";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
