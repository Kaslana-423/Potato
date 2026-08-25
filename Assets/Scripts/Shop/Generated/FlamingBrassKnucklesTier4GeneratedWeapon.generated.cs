public sealed class FlamingBrassKnucklesTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.flaming_brass_knuckles.tier_4";
    public override string DisplayName => "Flaming Brass Knuckles";
    public override string Description => "Deals -/ 8x5 / 12x6 / 15x7 (100% ) burning damage";
    public override int BasePrice => 173;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 64f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 0.59f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 1f;
    public override float Knockback => 15f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Unarmed , Elemental";
    public override string SpecialEffects => "Deals -/ 8x5 / 12x6 / 15x7 (100% ) burning damage";
}
