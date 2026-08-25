public sealed class GhostFlintTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.ghost_flint.tier_1";
    public override string DisplayName => "Ghost Flint";
    public override string Description => "+1% Attack Speed for every 20 / 18 / 16 / 12 kills in a wave with this weapon";
    public override int BasePrice => 12;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 6f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.23f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Ethereal";
    public override string SpecialEffects => "+1% Attack Speed for every 20 / 18 / 16 / 12 kills in a wave with this weapon";
}
