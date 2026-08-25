public sealed class LightningShivTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.lightning_shiv.tier_3";
    public override string DisplayName => "Lightning Shiv";
    public override string Description => "Hitting an enemy spawns a lightning projectile flying towards another random enemy that bounces - / 1 / 2 / 3 times and inflicts 5 / 6 / 8 / 11 + (80% ) damage";
    public override int BasePrice => 66;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 9f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.86f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 8f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Precise , Elemental";
    public override string SpecialEffects => "Hitting an enemy spawns a lightning projectile flying towards another random enemy that bounces - / 1 / 2 / 3 times and inflicts 5 / 6 / 8 / 11 + (80% ) damage";
}
