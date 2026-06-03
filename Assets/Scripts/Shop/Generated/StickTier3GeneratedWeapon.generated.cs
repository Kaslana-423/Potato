public sealed class StickTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.stick.tier_3";
    public override string DisplayName => "Stick";
    public override string Description => "Deals +4 / +6 / +8 / +10 base damage for every additional stick you have";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 10f;
    public override float AttackCooldown => 1.15f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Primitive";
    public override string SpecialEffects => "Deals +4 / +6 / +8 / +10 base damage for every additional stick you have";
}
