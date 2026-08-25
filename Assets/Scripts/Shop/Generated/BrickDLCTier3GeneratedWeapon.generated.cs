public sealed class BrickDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.brick_dlc.tier_3";
    public override string DisplayName => "Brick (DLC)";
    public override string Description => "Has a 1% chance to break and drop 10 / 30 / 60 / 120 materials on hit";
    public override int BasePrice => 22;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 90f;
    public override string DamageScalingText => "+80% +80%";
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
