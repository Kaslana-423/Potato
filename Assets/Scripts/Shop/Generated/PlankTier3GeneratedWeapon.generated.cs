public sealed class PlankTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.plank.tier_3";
    public override string DisplayName => "Plank";
    public override string Description => "Hits have 25% / 30% / 35% / 40% chance to explode.";
    public override int BasePrice => 61;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 20f;
    public override float AttackCooldown => 1.06f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Explosive , Elemental";
    public override string SpecialEffects => "Hits have 25% / 30% / 35% / 40% chance to explode.";
}
