public sealed class SharpToothTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sharp_tooth.tier_3";
    public override string DisplayName => "Sharp Tooth";
    public override string Description => "+1 % Life Steal for every 25% / 20% / 15% / 10% of missing health";
    public override int BasePrice => 52;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 11f;
    public override float AttackCooldown => 1f;
    public override float AttackRange => 170f;
    public override string ClassTags => "Primitive , Precise";
    public override string SpecialEffects => "+1 % Life Steal for every 25% / 20% / 15% / 10% of missing health";
}
