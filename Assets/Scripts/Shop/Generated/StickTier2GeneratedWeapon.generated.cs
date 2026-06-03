public sealed class StickTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.stick.tier_2";
    public override string DisplayName => "Stick";
    public override string Description => "Deals +4 / +6 / +8 / +10 base damage for every additional stick you have";
    public override int BasePrice => 22;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 9f;
    public override float AttackCooldown => 1.22f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Primitive";
    public override string SpecialEffects => "Deals +4 / +6 / +8 / +10 base damage for every additional stick you have";
}
