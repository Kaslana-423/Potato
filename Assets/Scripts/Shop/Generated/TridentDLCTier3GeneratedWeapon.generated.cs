public sealed class TridentDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.trident_dlc.tier_3";
    public override string DisplayName => "Trident (DLC)";
    public override string Description => "+30% / +40% / +50% damage against targets above 80% health";
    public override int BasePrice => 96;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 50f;
    public override float AttackCooldown => 1.5f;
    public override float AttackRange => 325f;
    public override string ClassTags => "Naval , Medieval";
    public override string SpecialEffects => "+30% / +40% / +50% damage against targets above 80% health";
}
