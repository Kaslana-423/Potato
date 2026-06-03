public sealed class StickTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.stick.tier_4";
    public override string DisplayName => "Stick";
    public override string Description => "Deals +4 / +6 / +8 / +10 base damage for every additional stick you have";
    public override int BasePrice => 91;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 12f;
    public override float AttackCooldown => 1.09f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Primitive";
    public override string SpecialEffects => "Deals +4 / +6 / +8 / +10 base damage for every additional stick you have";
}
