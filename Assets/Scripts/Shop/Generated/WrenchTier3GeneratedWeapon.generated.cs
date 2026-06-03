public sealed class WrenchTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.wrench.tier_3";
    public override string DisplayName => "Wrench";
    public override string Description => "Spawns a Turret / Incendiary Turret / Laser Turret / Explosive Turret";
    public override int BasePrice => 74;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 20f;
    public override float AttackCooldown => 1.55f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Tool";
    public override string SpecialEffects => "Spawns a Turret / Incendiary Turret / Laser Turret / Explosive Turret";
}
