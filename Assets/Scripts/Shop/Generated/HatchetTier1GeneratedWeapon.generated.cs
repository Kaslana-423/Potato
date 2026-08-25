public sealed class HatchetTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.hatchet.tier_1";
    public override string DisplayName => "Hatchet";
    public override string Description => "";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 5f;
    public override string DamageScalingText => "100% 15%";
    public override string DamageScalingStats => "Melee Damage,Attack Speed";
    public override float AttackCooldown => 0.68f;
    public override float AttackRange => 125f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive";
    public override string SpecialEffects => "";
}
