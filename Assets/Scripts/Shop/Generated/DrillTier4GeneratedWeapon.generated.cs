public sealed class DrillTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.drill.tier_4";
    public override string DisplayName => "Drill";
    public override string Description => "100% chance to gain 1 material when killing an enemy with a critical hit. +10 % Attack Speed every 5 seconds until the end of the wave";
    public override int BasePrice => 250;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 10f;
    public override float AttackCooldown => 0.45f;
    public override float AttackRange => 100f;
    public override string ClassTags => "Legendary , Precise";
    public override string SpecialEffects => "100% chance to gain 1 material when killing an enemy with a critical hit. +10 % Attack Speed every 5 seconds until the end of the wave";
}
