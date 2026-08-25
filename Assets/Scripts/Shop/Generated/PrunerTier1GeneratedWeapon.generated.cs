public sealed class PrunerTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.pruner.tier_1";
    public override string DisplayName => "Pruner";
    public override string Description => "Spawns a garden that creates a fruit every 15 / 14 / 12 / 10 seconds";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 10f;
    public override string DamageScalingText => "50%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.14f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.25f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Support";
    public override string SpecialEffects => "Spawns a garden that creates a fruit every 15 / 14 / 12 / 10 seconds";
}
