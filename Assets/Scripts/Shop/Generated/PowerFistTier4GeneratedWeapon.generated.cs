public sealed class PowerFistTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.power_fist.tier_4";
    public override string DisplayName => "Power Fist";
    public override string Description => "Hits have 25% / 50% chance to explode.";
    public override int BasePrice => 221;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 60f;
    public override float AttackCooldown => 0.59f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Unarmed , Explosive";
    public override string SpecialEffects => "Hits have 25% / 50% chance to explode.";
}
