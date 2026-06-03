public sealed class PrunerTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.pruner.tier_4";
    public override string DisplayName => "Pruner";
    public override string Description => "Spawns a garden that creates a fruit every 15 / 14 / 12 / 10 seconds";
    public override int BasePrice => 95;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 25f;
    public override float AttackCooldown => 0.89f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Support";
    public override string SpecialEffects => "Spawns a garden that creates a fruit every 15 / 14 / 12 / 10 seconds";
}
