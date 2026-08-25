public sealed class MaceDLCTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.mace_dlc.tier_2";
    public override string DisplayName => "Mace (DLC)";
    public override string Description => "-3% / -6% / -10% Attack Speed";
    public override int BasePrice => 46;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 40f;
    public override string DamageScalingText => "+100% -50%";
    public override string DamageScalingStats => "Melee Damage,Attack Speed";
    public override float AttackCooldown => 1.39f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Heavy , Medieval";
    public override string SpecialEffects => "-3% / -6% / -10% Attack Speed";
}
