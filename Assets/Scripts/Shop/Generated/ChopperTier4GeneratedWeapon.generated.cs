public sealed class ChopperTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.chopper.tier_4";
    public override string DisplayName => "Chopper";
    public override string Description => "+1 / +1 / +1 / +2 health healed from consumables";
    public override int BasePrice => 122;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 30f;
    public override string DamageScalingText => "50% 30%";
    public override string DamageScalingStats => "Melee Damage,Max HP";
    public override float AttackCooldown => 0.89f;
    public override float AttackRange => 180f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 25f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blade";
    public override string SpecialEffects => "+1 / +1 / +1 / +2 health healed from consumables";
}
