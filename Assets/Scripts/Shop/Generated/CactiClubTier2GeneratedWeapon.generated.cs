public sealed class CactiClubTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.cacti_club.tier_2";
    public override string DisplayName => "Cacti Club";
    public override string Description => "Hitting an enemy spawns 3 / 4 / 5 / 6 projectiles dealing 50% / 60% / 70% / 80% damage";
    public override int BasePrice => 39;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 20f;
    public override string DamageScalingText => "85%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.58f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 8f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive , Heavy";
    public override string SpecialEffects => "Hitting an enemy spawns 3 / 4 / 5 / 6 projectiles dealing 50% / 60% / 70% / 80% damage";
}
