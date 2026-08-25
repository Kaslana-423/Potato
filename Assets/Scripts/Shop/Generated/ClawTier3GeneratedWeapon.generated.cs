public sealed class ClawTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.claw.tier_3";
    public override string DisplayName => "Claw";
    public override string Description => "";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 15f;
    public override string DamageScalingText => "25% 50%";
    public override string DamageScalingStats => "Attack Speed,Melee Damage";
    public override float AttackCooldown => 0.69f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2.3f;
    public override float CritChance => 20f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Unarmed , Precise";
    public override string SpecialEffects => "";
}
