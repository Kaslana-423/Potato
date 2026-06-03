public sealed class BrickDlcWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.brick_dlc.tier_1";
    public override string DisplayName => "Brick (DLC)";
    public override string Description => "A cheap blunt weapon with a small chance to drop materials on hit.";
    public override int BasePrice => 6;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 30f;
    public override float AttackCooldown => 1.39f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Blunt";
    public override string SpecialEffects => "1% chance to break and drop materials on hit.";
}
