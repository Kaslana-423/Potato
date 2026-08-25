public sealed class CircularSawTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.circular_saw.tier_2";
    public override string DisplayName => "Circular Saw";
    public override string Description => "";
    public override int BasePrice => 46;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 10f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.72f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 5f;
    public override float Knockback => 0f;
    public override float LifeSteal => 45f;
    public override string ClassTags => "Blade , Medical";
    public override string SpecialEffects => "";
}
