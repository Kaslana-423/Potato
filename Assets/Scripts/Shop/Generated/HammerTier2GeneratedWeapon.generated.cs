public sealed class HammerTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.hammer.tier_2";
    public override string DisplayName => "Hammer";
    public override string Description => "+2 / +4 / +6 Knockback";
    public override int BasePrice => 51;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 35f;
    public override float AttackCooldown => 1.67f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Blunt , Heavy";
    public override string SpecialEffects => "+2 / +4 / +6 Knockback";
}
