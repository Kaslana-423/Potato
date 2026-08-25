public sealed class SpikyShieldTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.spiky_shield.tier_2";
    public override string DisplayName => "Spiky Shield";
    public override string Description => "";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 15f;
    public override string DamageScalingText => "125%";
    public override string DamageScalingStats => "Armor";
    public override float AttackCooldown => 1.16f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 20f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Medieval , Blunt";
    public override string SpecialEffects => "";
}
