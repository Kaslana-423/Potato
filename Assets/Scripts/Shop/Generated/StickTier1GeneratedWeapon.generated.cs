public sealed class StickTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.stick.tier_1";
    public override string DisplayName => "Stick";
    public override string Description => "Deals +4 / +6 / +8 / +10 base damage for every additional stick you have";
    public override int BasePrice => 10;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 8f;
    public override string DamageScalingText => "100%";
    public override string DamageScalingStats => "Melee Damage";
    public override float AttackCooldown => 1.29f;
    public override float AttackRange => 175f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 0f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Primitive";
    public override string SpecialEffects => "Deals +4 / +6 / +8 / +10 base damage for every additional stick you have";
    public override string IconResourcePath => "Weapon/stick";
    public override string RuntimePrefabResourcePath => "Weapon/Prefabs/StartingStickWeapon";
    public override string RuntimeSpriteResourcePath => "Weapon/stick";
}
