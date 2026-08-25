public sealed class VorpalSwordTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.vorpal_sword.tier_2";
    public override string DisplayName => "Vorpal Sword";
    public override string Description => "Alternates between thrusting and sweeping attacks -/ 1% / 2% / 3% chance to one shot the target when hitting it";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 20f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.11f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blade , Medieval";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks -/ 1% / 2% / 3% chance to one shot the target when hitting it";
}
