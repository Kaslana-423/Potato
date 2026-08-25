public sealed class GhostAxeTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.ghost_axe.tier_3";
    public override string DisplayName => "Ghost Axe";
    public override string Description => "+1% Damage for every 20 / 18 / 16 / 12 kills in a wave with this weapon";
    public override int BasePrice => 74;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 24f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.58f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 2f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Ethereal";
    public override string SpecialEffects => "+1% Damage for every 20 / 18 / 16 / 12 kills in a wave with this weapon";
}
