public sealed class MaceDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.mace_dlc.tier_3";
    public override string DisplayName => "Mace (DLC)";
    public override string Description => "-3% / -6% / -10% Attack Speed";
    public override int BasePrice => 92;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 60f;
    public override float AttackCooldown => 1.31f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Heavy , Medieval";
    public override string SpecialEffects => "-3% / -6% / -10% Attack Speed";
}
