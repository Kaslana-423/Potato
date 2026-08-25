public sealed class ClawTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.claw.tier_1";
    public override string DisplayName => "Claw";
    public override string Description => "";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 5f;
    public override string DamageScalingText => "15% 50%";
    public override string DamageScalingStats => "Attack Speed,Melee Damage";
    public override float AttackCooldown => 0.78f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 10f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Unarmed , Precise";
    public override string SpecialEffects => "";
}
