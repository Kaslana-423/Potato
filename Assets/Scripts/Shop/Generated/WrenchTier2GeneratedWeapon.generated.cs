public sealed class WrenchTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.wrench.tier_2";
    public override string DisplayName => "Wrench";
    public override string Description => "Spawns a Turret / Incendiary Turret / Laser Turret / Explosive Turret";
    public override int BasePrice => 39;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 16f;
    public override float AttackCooldown => 1.64f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Tool";
    public override string SpecialEffects => "Spawns a Turret / Incendiary Turret / Laser Turret / Explosive Turret";
}
