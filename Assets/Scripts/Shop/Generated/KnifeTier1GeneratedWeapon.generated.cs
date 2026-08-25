public sealed class KnifeTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.knife.tier_1";
    public override string DisplayName => "Knife";
    public override string Description => "";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 6f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.01f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2.5f;
    public override float CritChance => 20f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Precise";
    public override string SpecialEffects => "";
}
