public sealed class HatchetTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.hatchet.tier_4";
    public override string DisplayName => "Hatchet";
    public override string Description => "";
    public override int BasePrice => 122;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 40f;
    public override string DamageScalingText => "100% 15%";
    public override string DamageScalingStats => "Melee Damage,Attack Speed";
    public override float AttackCooldown => 0.58f;
    public override float AttackRange => 125f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive";
    public override string SpecialEffects => "";
}
