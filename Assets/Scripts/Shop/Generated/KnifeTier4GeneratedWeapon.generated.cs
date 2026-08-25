public sealed class KnifeTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.knife.tier_4";
    public override string DisplayName => "Knife";
    public override string Description => "";
    public override int BasePrice => 122;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 20f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.78f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 4f;
    public override float CritChance => 50f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Precise";
    public override string SpecialEffects => "";
}
