public sealed class JoustingLanceTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.jousting_lance.tier_2";
    public override string DisplayName => "Jousting Lance";
    public override string Description => "+2 / +3 / +4 / +5 % Speed -10 / -15 / -20 / -25 % Damage while standing still";
    public override int BasePrice => 36;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 25f;
    public override string DamageScalingText => "50% 35%";
    public override string DamageScalingStats => "Melee Damage,Speed";
    public override float AttackCooldown => 1.5f;
    public override float AttackRange => 250f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Medieval";
    public override string SpecialEffects => "+2 / +3 / +4 / +5 % Speed -10 / -15 / -20 / -25 % Damage while standing still";
}
