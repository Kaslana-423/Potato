public sealed class PlasmaSledgeTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.plasma_sledge.tier_3";
    public override string DisplayName => "Plasma Sledge";
    public override string Description => "Hits have 25% / 50% chance to explode.";
    public override int BasePrice => 136;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 80f;
    public override string DamageScalingText => "150% 150%";
    public override string DamageScalingStats => "Melee Damage,Elemental Damage";
    public override float AttackCooldown => 1.55f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 1.75f;
    public override float CritChance => 3f;
    public override float Knockback => 30f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Elemental , Explosive";
    public override string SpecialEffects => "Hits have 25% / 50% chance to explode.";
}
