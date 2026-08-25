public sealed class SpearTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.spear.tier_3";
    public override string DisplayName => "Spear";
    public override string Description => "";
    public override int BasePrice => 74;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 40f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.28f;
    public override float AttackRange => 400f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive";
    public override string SpecialEffects => "";
}
