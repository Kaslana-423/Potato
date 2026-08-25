public sealed class ThiefDaggerTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.thief_dagger.tier_1";
    public override string DisplayName => "Thief Dagger";
    public override string Description => "50% / 56% / 62% / 80% to gain 1 material when killing an enemy with a critical hit with this weapon";
    public override int BasePrice => 12;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 6f;
    public override string DamageScalingText => "50%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.01f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 20f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Precise";
    public override string SpecialEffects => "50% / 56% / 62% / 80% to gain 1 material when killing an enemy with a critical hit with this weapon";
}
