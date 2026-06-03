public sealed class PrunerTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.pruner.tier_3";
    public override string DisplayName => "Pruner";
    public override string Description => "Spawns a garden that creates a fruit every 15 / 14 / 12 / 10 seconds";
    public override int BasePrice => 52;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 20f;
    public override float AttackCooldown => 0.98f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Support";
    public override string SpecialEffects => "Spawns a garden that creates a fruit every 15 / 14 / 12 / 10 seconds";
}
