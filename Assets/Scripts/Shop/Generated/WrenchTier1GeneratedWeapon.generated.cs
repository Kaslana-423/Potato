public sealed class WrenchTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.wrench.tier_1";
    public override string DisplayName => "Wrench";
    public override string Description => "Spawns a Turret / Incendiary Turret / Laser Turret / Explosive Turret";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override float Damage => 12f;
    public override float AttackCooldown => 1.7f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Tool";
    public override string SpecialEffects => "Spawns a Turret / Incendiary Turret / Laser Turret / Explosive Turret";
}
