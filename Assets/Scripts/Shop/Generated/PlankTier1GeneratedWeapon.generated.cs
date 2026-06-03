public sealed class PlankTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.plank.tier_1";
    public override string DisplayName => "Plank";
    public override string Description => "Hits have 25% / 30% / 35% / 40% chance to explode.";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override float Damage => 10f;
    public override float AttackCooldown => 1.23f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Explosive , Elemental";
    public override string SpecialEffects => "Hits have 25% / 30% / 35% / 40% chance to explode.";
}
