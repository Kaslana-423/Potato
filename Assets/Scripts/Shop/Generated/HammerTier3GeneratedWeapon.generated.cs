public sealed class HammerTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.hammer.tier_3";
    public override string DisplayName => "Hammer";
    public override string Description => "+2 / +4 / +6 Knockback";
    public override int BasePrice => 95;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 70f;
    public override string DamageScalingText => "175%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.59f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 1.75f;
    public override float CritChance => 3f;
    public override float Knockback => 40f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blunt , Heavy";
    public override string SpecialEffects => "+2 / +4 / +6 Knockback";
}
