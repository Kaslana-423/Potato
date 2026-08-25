public sealed class ScissorsTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.scissors.tier_2";
    public override string DisplayName => "Scissors";
    public override string Description => "";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 10f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.98f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2.15f;
    public override float CritChance => 12f;
    public override float Knockback => 2f;
    public override float LifeSteal => 45f;
    public override string ClassTags => "Medical , Precise";
    public override string SpecialEffects => "";
}
