public sealed class ChopperTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.chopper.tier_2";
    public override string DisplayName => "Chopper";
    public override string Description => "+1 / +1 / +1 / +2 health healed from consumables";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 12f;
    public override float AttackCooldown => 0.99f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Blade";
    public override string SpecialEffects => "+1 / +1 / +1 / +2 health healed from consumables";
}
