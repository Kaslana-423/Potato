public sealed class CircularSawTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.circular_saw.tier_4";
    public override string DisplayName => "Circular Saw";
    public override string Description => "";
    public override int BasePrice => 173;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 25f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.62f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 20f;
    public override float Knockback => 0f;
    public override float LifeSteal => 60f;
    public override string ClassTags => "Blade , Medical";
    public override string SpecialEffects => "";
}
