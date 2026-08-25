public sealed class ExcaliburTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.excalibur.tier_4";
    public override string DisplayName => "Excalibur";
    public override string Description => "Alternates between thrusting and sweeping attacks. -3 Armor for every weapon you have";
    public override int BasePrice => 230;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 200f;
    public override string DamageScalingText => "200% 200%";
    public override string DamageScalingStats => "Melee Damage,Max HP";
    public override float AttackCooldown => 0.66f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 2.5f;
    public override float CritChance => 10f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Legendary , Blade";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks. -3 Armor for every weapon you have";
}
