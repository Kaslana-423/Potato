public sealed class FistTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.fist.tier_2";
    public override string DisplayName => "Fist";
    public override string Description => "";
    public override int BasePrice => 22;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 16f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.73f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 1f;
    public override float Knockback => 15f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Unarmed";
    public override string SpecialEffects => "";
}
