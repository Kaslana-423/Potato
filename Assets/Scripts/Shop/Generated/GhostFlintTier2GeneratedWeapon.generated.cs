public sealed class GhostFlintTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.ghost_flint.tier_2";
    public override string DisplayName => "Ghost Flint";
    public override string Description => "+1% Attack Speed for every 20 / 18 / 16 / 12 kills in a wave with this weapon";
    public override int BasePrice => 26;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 9f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.17f;
    public override float AttackRange => 160f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Ethereal";
    public override string SpecialEffects => "+1% Attack Speed for every 20 / 18 / 16 / 12 kills in a wave with this weapon";
}
