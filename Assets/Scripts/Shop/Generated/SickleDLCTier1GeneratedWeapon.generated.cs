public sealed class SickleDLCTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sickle_dlc.tier_1";
    public override string DisplayName => "Sickle (DLC)";
    public override string Description => "+20% / +30% / +40% / +50% damage against targets below 30% health";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override float Damage => 5f;
    public override float AttackCooldown => 0.87f;
    public override float AttackRange => 125f;
    public override string ClassTags => "Support";
    public override string SpecialEffects => "+20% / +30% / +40% / +50% damage against targets below 30% health";
}
