public sealed class BrickDLCTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.brick_dlc.tier_4";
    public override string DisplayName => "Brick (DLC)";
    public override string Description => "Has a 1% chance to break and drop 10 / 30 / 60 / 120 materials on hit";
    public override int BasePrice => 40;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 120f;
    public override string DamageScalingText => "+100% +100%";
    public override string DamageScalingStats => "Melee Damage,Engineering";
    public override float AttackCooldown => 1.39f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blunt";
    public override string SpecialEffects => "Has a 1% chance to break and drop 10 / 30 / 60 / 120 materials on hit";
}
