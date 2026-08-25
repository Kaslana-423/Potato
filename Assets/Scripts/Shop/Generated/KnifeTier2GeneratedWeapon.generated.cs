public sealed class KnifeTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.knife.tier_2";
    public override string DisplayName => "Knife";
    public override string Description => "";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 9f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.93f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 3f;
    public override float CritChance => 30f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Precise";
    public override string SpecialEffects => "";
}
