public sealed class TridentDLCTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.trident_dlc.tier_2";
    public override string DisplayName => "Trident (DLC)";
    public override string Description => "+30% / +40% / +50% damage against targets above 80% health";
    public override int BasePrice => 52;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 30f;
    public override float AttackCooldown => 1.58f;
    public override float AttackRange => 325f;
    public override string ClassTags => "Naval , Medieval";
    public override string SpecialEffects => "+30% / +40% / +50% damage against targets above 80% health";
}
