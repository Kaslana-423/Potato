public sealed class SpoonDLCTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.spoon_dlc.tier_2";
    public override string DisplayName => "Spoon (DLC)";
    public override string Description => "Always crits when hitting burning targets";
    public override int BasePrice => 31;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 15f;
    public override string DamageScalingText => "+50% +20%";
    public override string DamageScalingStats => "Melee Damage,Max HP";
    public override float AttackCooldown => 0.99f;
    public override float AttackRange => 165f;
    public override float CritMultiplier => 2.25f;
    public override float CritChance => 0f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blunt";
    public override string SpecialEffects => "Always crits when hitting burning targets";
}
