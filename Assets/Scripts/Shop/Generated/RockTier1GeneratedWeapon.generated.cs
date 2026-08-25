public sealed class RockTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.rock.tier_1";
    public override string DisplayName => "Rock";
    public override string Description => "- / +1 Armor / +1 Armor, +2 Max HP / +2 Armor, +2 Max HP";
    public override int BasePrice => 10;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 20f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.68f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 10f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive , Blunt";
    public override string SpecialEffects => "- / +1 Armor / +1 Armor, +2 Max HP / +2 Armor, +2 Max HP";
}
