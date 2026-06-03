public sealed class ChopperTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.chopper.tier_3";
    public override string DisplayName => "Chopper";
    public override string Description => "+1 / +1 / +1 / +2 health healed from consumables";
    public override int BasePrice => 61;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 18f;
    public override float AttackCooldown => 0.96f;
    public override float AttackRange => 165f;
    public override string ClassTags => "Blade";
    public override string SpecialEffects => "+1 / +1 / +1 / +2 health healed from consumables";
}
