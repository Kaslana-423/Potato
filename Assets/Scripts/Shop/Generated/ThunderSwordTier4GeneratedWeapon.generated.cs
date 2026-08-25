public sealed class ThunderSwordTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.thunder_sword.tier_4";
    public override string DisplayName => "Thunder Sword";
    public override string Description => "Hitting an enemy spawns 2 / 4 projectiles that slow and damage enemies by 100%";
    public override int BasePrice => 238;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 60f;
    public override string DamageScalingText => "150% 150%";
    public override string DamageScalingStats => "Melee Damage,Elemental Damage";
    public override float AttackCooldown => 1.06f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blade , Elemental";
    public override string SpecialEffects => "Hitting an enemy spawns 2 / 4 projectiles that slow and damage enemies by 100%";
}
