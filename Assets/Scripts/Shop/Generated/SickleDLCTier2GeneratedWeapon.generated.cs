public sealed class SickleDLCTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sickle_dlc.tier_2";
    public override string DisplayName => "Sickle (DLC)";
    public override string Description => "+20% / +30% / +40% / +50% damage against targets below 30% health";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 8f;
    public override string DamageScalingText => "+80% +15%";
    public override string DamageScalingStats => "Melee Damage,Harvesting";
    public override float AttackCooldown => 0.82f;
    public override float AttackRange => 125f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Support";
    public override string SpecialEffects => "+20% / +30% / +40% / +50% damage against targets below 30% health";
}
