public sealed class LuteDLCTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.lute_dlc.tier_2";
    public override string DisplayName => "Lute (DLC)";
    public override string Description => "Enemies hit take 10% more damage for 3 seconds (max: 30% / 50% / 70% / 100% )";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 8f;
    public override string DamageScalingText => "+50% +15%";
    public override string DamageScalingStats => "Melee Damage,Luck";
    public override float AttackCooldown => 1.25f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Musical , Support";
    public override string SpecialEffects => "Enemies hit take 10% more damage for 3 seconds (max: 30% / 50% / 70% / 100% )";
}
