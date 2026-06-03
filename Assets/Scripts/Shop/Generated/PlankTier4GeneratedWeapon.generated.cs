public sealed class PlankTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.plank.tier_4";
    public override string DisplayName => "Plank";
    public override string Description => "Hits have 25% / 30% / 35% / 40% chance to explode.";
    public override int BasePrice => 122;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 25f;
    public override float AttackCooldown => 0.98f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Explosive , Elemental";
    public override string SpecialEffects => "Hits have 25% / 30% / 35% / 40% chance to explode.";
}
