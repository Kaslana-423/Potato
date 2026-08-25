public sealed class ScissorsTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.scissors.tier_3";
    public override string DisplayName => "Scissors";
    public override string Description => "";
    public override int BasePrice => 61;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 15f;
    public override string DamageScalingText => "80%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.94f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2.3f;
    public override float CritChance => 15f;
    public override float Knockback => 2f;
    public override float LifeSteal => 50f;
    public override string ClassTags => "Medical , Precise";
    public override string SpecialEffects => "";
}
