public sealed class SharpToothTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sharp_tooth.tier_2";
    public override string DisplayName => "Sharp Tooth";
    public override string Description => "+1 % Life Steal for every 25% / 20% / 15% / 10% of missing health";
    public override int BasePrice => 26;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 8f;
    public override float AttackCooldown => 1.07f;
    public override float AttackRange => 160f;
    public override string ClassTags => "Primitive , Precise";
    public override string SpecialEffects => "+1 % Life Steal for every 25% / 20% / 15% / 10% of missing health";
}
