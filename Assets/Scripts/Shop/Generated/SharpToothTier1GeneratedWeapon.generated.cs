public sealed class SharpToothTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sharp_tooth.tier_1";
    public override string DisplayName => "Sharp Tooth";
    public override string Description => "+1 % Life Steal for every 25% / 20% / 15% / 10% of missing health";
    public override int BasePrice => 12;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 5f;
    public override string DamageScalingText => "+50% +50%";
    public override string DamageScalingStats => "Melee Damage,Life Steal";
    public override float AttackCooldown => 1.14f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive , Precise";
    public override string SpecialEffects => "+1 % Life Steal for every 25% / 20% / 15% / 10% of missing health";
}
