public sealed class DrillTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.drill.tier_4";
    public override string DisplayName => "Drill";
    public override string Description => "100% chance to gain 1 material when killing an enemy with a critical hit. +10 % Attack Speed every 5 seconds until the end of the wave";
    public override int BasePrice => 250;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 10f;
    public override string DamageScalingText => "100% 100%";
    public override string DamageScalingStats => "Melee Damage,Engineering";
    public override float AttackCooldown => 0.45f;
    public override float AttackRange => 100f;
    public override float CritMultiplier => 2.5f;
    public override float CritChance => 50f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Legendary , Precise";
    public override string SpecialEffects => "100% chance to gain 1 material when killing an enemy with a critical hit. +10 % Attack Speed every 5 seconds until the end of the wave";
}
