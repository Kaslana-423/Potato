public sealed class PowerFistTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.power_fist.tier_3";
    public override string DisplayName => "Power Fist";
    public override string Description => "Hits have 25% / 50% chance to explode.";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 40f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.69f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 1f;
    public override float Knockback => 15f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Unarmed , Explosive";
    public override string SpecialEffects => "Hits have 25% / 50% chance to explode.";
}
