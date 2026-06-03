public sealed class PlankTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.plank.tier_2";
    public override string DisplayName => "Plank";
    public override string Description => "Hits have 25% / 30% / 35% / 40% chance to explode.";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 15f;
    public override float AttackCooldown => 1.14f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Explosive , Elemental";
    public override string SpecialEffects => "Hits have 25% / 30% / 35% / 40% chance to explode.";
}
