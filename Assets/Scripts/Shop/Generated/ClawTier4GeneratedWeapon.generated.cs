public sealed class ClawTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.claw.tier_4";
    public override string DisplayName => "Claw";
    public override string Description => "";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 25f;
    public override string DamageScalingText => "30% 50%";
    public override string DamageScalingStats => "Attack Speed,Melee Damage";
    public override float AttackCooldown => 0.61f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2.5f;
    public override float CritChance => 25f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Unarmed , Precise";
    public override string SpecialEffects => "";
}
