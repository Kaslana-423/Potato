public sealed class PrunerTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.pruner.tier_2";
    public override string DisplayName => "Pruner";
    public override string Description => "Spawns a garden that creates a fruit every 15 / 14 / 12 / 10 seconds";
    public override int BasePrice => 28;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 15f;
    public override float AttackCooldown => 1.06f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Support";
    public override string SpecialEffects => "Spawns a garden that creates a fruit every 15 / 14 / 12 / 10 seconds";
}
