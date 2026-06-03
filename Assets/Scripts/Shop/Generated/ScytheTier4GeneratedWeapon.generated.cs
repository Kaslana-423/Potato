public sealed class ScytheTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.scythe.tier_4";
    public override string DisplayName => "Scythe";
    public override string Description => "You take 3 damage per second (does not give invulnerability time). +3 % Damage when you take damage until the end of the wave";
    public override int BasePrice => 285;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 150f;
    public override float AttackCooldown => 0.72f;
    public override float AttackRange => 250f;
    public override string ClassTags => "Legendary , Ethereal";
    public override string SpecialEffects => "You take 3 damage per second (does not give invulnerability time). +3 % Damage when you take damage until the end of the wave";
}
