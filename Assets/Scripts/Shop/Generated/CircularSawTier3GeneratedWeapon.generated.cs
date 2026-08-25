public sealed class CircularSawTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.circular_saw.tier_3";
    public override string DisplayName => "Circular Saw";
    public override string Description => "";
    public override int BasePrice => 86;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 15f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.67f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 10f;
    public override float Knockback => 0f;
    public override float LifeSteal => 50f;
    public override string ClassTags => "Blade , Medical";
    public override string SpecialEffects => "";
}
