public sealed class LightningShivTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.lightning_shiv.tier_4";
    public override string DisplayName => "Lightning Shiv";
    public override string Description => "Hitting an enemy spawns a lightning projectile flying towards another random enemy that bounces - / 1 / 2 / 3 times and inflicts 5 / 6 / 8 / 11 + (80% ) damage";
    public override int BasePrice => 142;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 15f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.78f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 10f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Precise , Elemental";
    public override string SpecialEffects => "Hitting an enemy spawns a lightning projectile flying towards another random enemy that bounces - / 1 / 2 / 3 times and inflicts 5 / 6 / 8 / 11 + (80% ) damage";
}
