public sealed class HandTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.hand.tier_3";
    public override string DisplayName => "Hand";
    public override string Description => "+3 / +6 / +9 / +18 Harvesting";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 1f;
    public override string DamageScalingText => "50%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.86f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 1f;
    public override float Knockback => 30f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Unarmed , Support";
    public override string SpecialEffects => "+3 / +6 / +9 / +18 Harvesting";
}
