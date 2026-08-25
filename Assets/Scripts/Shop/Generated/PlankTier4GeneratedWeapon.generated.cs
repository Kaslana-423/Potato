public sealed class PlankTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.plank.tier_4";
    public override string DisplayName => "Plank";
    public override string Description => "Hits have 25% / 30% / 35% / 40% chance to explode.";
    public override int BasePrice => 122;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 25f;
    public override string DamageScalingText => "80% 80% 80%";
    public override string DamageScalingStats => "Melee Damage,Elemental Damage,Engineering";
    public override float AttackCooldown => 0.98f;
    public override float AttackRange => 150f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Explosive , Elemental";
    public override string SpecialEffects => "Hits have 25% / 30% / 35% / 40% chance to explode.";
}
