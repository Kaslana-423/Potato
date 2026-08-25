public sealed class SickleDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sickle_dlc.tier_3";
    public override string DisplayName => "Sickle (DLC)";
    public override string Description => "+20% / +30% / +40% / +50% damage against targets below 30% health";
    public override int BasePrice => 61;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 12f;
    public override string DamageScalingText => "+80% +20%";
    public override string DamageScalingStats => "Melee Damage,Harvesting";
    public override float AttackCooldown => 0.77f;
    public override float AttackRange => 125f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Support";
    public override string SpecialEffects => "+20% / +30% / +40% / +50% damage against targets below 30% health";
}
