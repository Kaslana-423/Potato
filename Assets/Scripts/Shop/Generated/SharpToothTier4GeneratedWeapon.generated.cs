public sealed class SharpToothTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sharp_tooth.tier_4";
    public override string DisplayName => "Sharp Tooth";
    public override string Description => "+1 % Life Steal for every 25% / 20% / 15% / 10% of missing health";
    public override int BasePrice => 105;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 15f;
    public override string DamageScalingText => "+50% +100%";
    public override string DamageScalingStats => "Melee Damage,Life Steal";
    public override float AttackCooldown => 0.93f;
    public override float AttackRange => 180f;
    public override float CritMultiplier => 2.5f;
    public override float CritChance => 12f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive , Precise";
    public override string SpecialEffects => "+1 % Life Steal for every 25% / 20% / 15% / 10% of missing health";
}
