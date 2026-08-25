public sealed class TorchTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.torch.tier_3";
    public override string DisplayName => "Torch";
    public override string Description => "Deals 3x3 / 5x5 / 8x6 / 12x9 (100% ) burning damage. Burning spreads to 0 / 0 / 1 / 1 additional nearby enemies.";
    public override int BasePrice => 45;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 1f;
    public override string DamageScalingText => "80% 80%";
    public override string DamageScalingStats => "Melee Damage,Elemental Damage";
    public override float AttackCooldown => 0.95f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 0f;
    public override float Knockback => 20f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive , Elemental";
    public override string SpecialEffects => "Deals 3x3 / 5x5 / 8x6 / 12x9 (100% ) burning damage. Burning spreads to 0 / 0 / 1 / 1 additional nearby enemies.";
}
