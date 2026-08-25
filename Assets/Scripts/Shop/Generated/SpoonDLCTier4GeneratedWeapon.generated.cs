public sealed class SpoonDLCTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.spoon_dlc.tier_4";
    public override string DisplayName => "Spoon (DLC)";
    public override string Description => "Always crits when hitting burning targets";
    public override int BasePrice => 122;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 25f;
    public override string DamageScalingText => "+50% +25%";
    public override string DamageScalingStats => "Melee Damage,Max HP";
    public override float AttackCooldown => 0.86f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 3f;
    public override float CritChance => 0f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blunt";
    public override string SpecialEffects => "Always crits when hitting burning targets";
}
