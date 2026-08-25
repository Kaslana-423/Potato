public sealed class ThunderSwordTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.thunder_sword.tier_3";
    public override string DisplayName => "Thunder Sword";
    public override string Description => "Hitting an enemy spawns 2 / 4 projectiles that slow and damage enemies by 100%";
    public override int BasePrice => 119;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 30f;
    public override string DamageScalingText => "125% 125%";
    public override string DamageScalingStats => "Melee Damage,Elemental Damage";
    public override float AttackCooldown => 1.21f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blade , Elemental";
    public override string SpecialEffects => "Hitting an enemy spawns 2 / 4 projectiles that slow and damage enemies by 100%";
}
