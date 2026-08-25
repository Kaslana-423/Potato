public sealed class ClawTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.claw.tier_2";
    public override string DisplayName => "Claw";
    public override string Description => "";
    public override int BasePrice => 28;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 10f;
    public override string DamageScalingText => "20% 50%";
    public override string DamageScalingStats => "Attack Speed,Melee Damage";
    public override float AttackCooldown => 0.74f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2.15f;
    public override float CritChance => 15f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Unarmed , Precise";
    public override string SpecialEffects => "";
}
