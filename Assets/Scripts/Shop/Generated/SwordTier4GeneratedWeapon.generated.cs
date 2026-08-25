public sealed class SwordTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sword.tier_4";
    public override string DisplayName => "Sword";
    public override string Description => "Alternates between thrusting and sweeping attacks";
    public override int BasePrice => 190;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 60f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.98f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blade , Medieval";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks";
}
