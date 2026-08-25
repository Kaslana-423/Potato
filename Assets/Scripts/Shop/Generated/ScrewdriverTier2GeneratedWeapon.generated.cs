public sealed class ScrewdriverTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.screwdriver.tier_2";
    public override string DisplayName => "Screwdriver";
    public override string Description => "A landmine spawns every 12s / 9s / 6s / 3s dealing 10(100% ) damage in an area";
    public override int BasePrice => 22;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 12f;
    public override string DamageScalingText => "50% 50%";
    public override string DamageScalingStats => "Melee Damage,Engineering";
    public override float AttackCooldown => 1f;
    public override float AttackRange => 125f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 15f;
    public override float Knockback => 3f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Tool";
    public override string SpecialEffects => "A landmine spawns every 12s / 9s / 6s / 3s dealing 10(100% ) damage in an area";
}
