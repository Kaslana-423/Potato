public sealed class LightningShivTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.lightning_shiv.tier_2";
    public override string DisplayName => "Lightning Shiv";
    public override string Description => "Hitting an enemy spawns a lightning projectile flying towards another random enemy that bounces - / 1 / 2 / 3 times and inflicts 5 / 6 / 8 / 11 + (80% ) damage";
    public override int BasePrice => 36;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 6f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.93f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 6f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Precise , Elemental";
    public override string SpecialEffects => "Hitting an enemy spawns a lightning projectile flying towards another random enemy that bounces - / 1 / 2 / 3 times and inflicts 5 / 6 / 8 / 11 + (80% ) damage";
}
