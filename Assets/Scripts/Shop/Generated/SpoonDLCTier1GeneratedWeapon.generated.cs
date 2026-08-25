public sealed class SpoonDLCTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.spoon_dlc.tier_1";
    public override string DisplayName => "Spoon (DLC)";
    public override string Description => "Always crits when hitting burning targets";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 10f;
    public override string DamageScalingText => "+50% +15%";
    public override string DamageScalingStats => "Melee Damage,Max HP";
    public override float AttackCooldown => 1.06f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 0f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blunt";
    public override string SpecialEffects => "Always crits when hitting burning targets";
}
