public sealed class KnifeTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.knife.tier_3";
    public override string DisplayName => "Knife";
    public override string Description => "";
    public override int BasePrice => 61;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 12f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.86f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 3.5f;
    public override float CritChance => 40f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Precise";
    public override string SpecialEffects => "";
}
