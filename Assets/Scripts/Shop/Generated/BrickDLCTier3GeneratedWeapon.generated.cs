public sealed class BrickDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.brick_dlc.tier_3";
    public override string DisplayName => "Brick (DLC)";
    public override string Description => "Has a 1% chance to break and drop 10 / 30 / 60 / 120 materials on hit";
    public override int BasePrice => 22;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 90f;
    public override float AttackCooldown => 1.39f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Blunt";
    public override string SpecialEffects => "Has a 1% chance to break and drop 10 / 30 / 60 / 120 materials on hit";
}
