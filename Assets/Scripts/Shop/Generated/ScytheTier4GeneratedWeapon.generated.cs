public sealed class ScytheTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.scythe.tier_4";
    public override string DisplayName => "Scythe";
    public override string Description => "You take 3 damage per second (does not give invulnerability time). +3 % Damage when you take damage until the end of the wave";
    public override int BasePrice => 285;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 150f;
    public override string DamageScalingText => "150% 100%";
    public override string DamageScalingStats => "Melee Damage,Life Steal";
    public override float AttackCooldown => 0.72f;
    public override float AttackRange => 250f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 100f;
    public override string ClassTags => "Legendary , Ethereal";
    public override string SpecialEffects => "You take 3 damage per second (does not give invulnerability time). +3 % Damage when you take damage until the end of the wave";
}
