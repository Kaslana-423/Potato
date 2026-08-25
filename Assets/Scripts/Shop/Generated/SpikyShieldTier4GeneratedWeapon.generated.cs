public sealed class SpikyShieldTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.spiky_shield.tier_4";
    public override string DisplayName => "Spiky Shield";
    public override string Description => "";
    public override int BasePrice => 122;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 30f;
    public override string DamageScalingText => "200%";
    public override string DamageScalingStats => "Armor";
    public override float AttackCooldown => 1.01f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 20f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Medieval , Blunt";
    public override string SpecialEffects => "";
}
