public sealed class SpoonDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.spoon_dlc.tier_3";
    public override string DisplayName => "Spoon (DLC)";
    public override string Description => "Always crits when hitting burning targets";
    public override int BasePrice => 61;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 20f;
    public override string DamageScalingText => "+50% +25%";
    public override string DamageScalingStats => "Melee Damage,Max HP";
    public override float AttackCooldown => 0.93f;
    public override float AttackRange => 180f;
    public override float CritMultiplier => 2.5f;
    public override float CritChance => 0f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blunt";
    public override string SpecialEffects => "Always crits when hitting burning targets";
}
