public sealed class LuteDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.lute_dlc.tier_3";
    public override string DisplayName => "Lute (DLC)";
    public override string Description => "Enemies hit take 10% more damage for 3 seconds (max: 30% / 50% / 70% / 100% )";
    public override int BasePrice => 61;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 12f;
    public override float AttackCooldown => 1.2f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Musical , Support";
    public override string SpecialEffects => "Enemies hit take 10% more damage for 3 seconds (max: 30% / 50% / 70% / 100% )";
}
