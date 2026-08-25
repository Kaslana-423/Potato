public sealed class SpearTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.spear.tier_2";
    public override string DisplayName => "Spear";
    public override string Description => "";
    public override int BasePrice => 39;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 25f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.4f;
    public override float AttackRange => 375f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive";
    public override string SpecialEffects => "";
}
