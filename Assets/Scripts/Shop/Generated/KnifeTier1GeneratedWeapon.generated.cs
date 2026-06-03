public sealed class KnifeTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.knife.tier_1";
    public override string DisplayName => "Knife";
    public override string Description => "";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override float Damage => 6f;
    public override float AttackCooldown => 1.01f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Precise";
    public override string SpecialEffects => "";
}
