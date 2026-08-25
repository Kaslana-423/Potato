public sealed class HikingPoleDLCTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.hiking_pole_dlc.tier_4";
    public override string DisplayName => "Hiking Pole (DLC)";
    public override string Description => "+1 Range for every 70 / 60 / 50 / 40 steps you take during a wave";
    public override int BasePrice => 130;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 30f;
    public override string DamageScalingText => "+50% +20%";
    public override string DamageScalingStats => "Melee Damage,Range";
    public override float AttackCooldown => 1.17f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Support";
    public override string SpecialEffects => "+1 Range for every 70 / 60 / 50 / 40 steps you take during a wave";
}
