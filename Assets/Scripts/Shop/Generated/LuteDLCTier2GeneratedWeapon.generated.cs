public sealed class LuteDLCTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.lute_dlc.tier_2";
    public override string DisplayName => "Lute (DLC)";
    public override string Description => "Enemies hit take 10% more damage for 3 seconds (max: 30% / 50% / 70% / 100% )";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 8f;
    public override float AttackCooldown => 1.25f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Musical , Support";
    public override string SpecialEffects => "Enemies hit take 10% more damage for 3 seconds (max: 30% / 50% / 70% / 100% )";
}
