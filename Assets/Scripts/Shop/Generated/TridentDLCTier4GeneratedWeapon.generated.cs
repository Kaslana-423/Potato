public sealed class TridentDLCTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.trident_dlc.tier_4";
    public override string DisplayName => "Trident (DLC)";
    public override string Description => "+30% / +40% / +50% damage against targets above 80% health";
    public override int BasePrice => 200;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 80f;
    public override string DamageScalingText => "+100% +50%";
    public override string DamageScalingStats => "Melee Damage,Curse";
    public override float AttackCooldown => 1.41f;
    public override float AttackRange => 325f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Naval , Medieval";
    public override string SpecialEffects => "+30% / +40% / +50% damage against targets above 80% health";
}
