public sealed class QuarterstaffTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.quarterstaff.tier_4";
    public override string DisplayName => "Quarterstaff";
    public override string Description => "Alternates between thrusting and sweeping attacks +2% XP Gain / +5% XP Gain / +9% XP Gain / +15% XP Gain";
    public override int BasePrice => 130;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 30f;
    public override string DamageScalingText => "125% 100%";
    public override string DamageScalingStats => "Level,Melee Damage";
    public override float AttackCooldown => 0.92f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive , Medieval";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks +2% XP Gain / +5% XP Gain / +9% XP Gain / +15% XP Gain";
}
