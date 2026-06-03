public sealed class HandTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.hand.tier_2";
    public override string DisplayName => "Hand";
    public override string Description => "+3 / +6 / +9 / +18 Harvesting";
    public override int BasePrice => 22;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 1f;
    public override float AttackCooldown => 0.93f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Unarmed , Support";
    public override string SpecialEffects => "+3 / +6 / +9 / +18 Harvesting";
}
