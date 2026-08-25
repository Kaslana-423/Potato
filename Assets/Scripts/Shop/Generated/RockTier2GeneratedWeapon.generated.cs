public sealed class RockTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.rock.tier_2";
    public override string DisplayName => "Rock";
    public override string Description => "- / +1 Armor / +1 Armor, +2 Max HP / +2 Armor, +2 Max HP";
    public override int BasePrice => 22;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 35f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.61f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 15f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive , Blunt";
    public override string SpecialEffects => "- / +1 Armor / +1 Armor, +2 Max HP / +2 Armor, +2 Max HP";
}
