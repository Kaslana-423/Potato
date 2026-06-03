public sealed class BrickDLCTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.brick_dlc.tier_2";
    public override string DisplayName => "Brick (DLC)";
    public override string Description => "Has a 1% chance to break and drop 10 / 30 / 60 / 120 materials on hit";
    public override int BasePrice => 14;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 60f;
    public override float AttackCooldown => 1.39f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Blunt";
    public override string SpecialEffects => "Has a 1% chance to break and drop 10 / 30 / 60 / 120 materials on hit";
}
