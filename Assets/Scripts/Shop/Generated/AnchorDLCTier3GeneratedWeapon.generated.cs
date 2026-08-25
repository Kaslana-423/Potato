public sealed class AnchorDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.anchor_dlc.tier_3";
    public override string DisplayName => "Anchor (DLC)";
    public override string Description => "";
    public override int BasePrice => 95;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 70f;
    public override string DamageScalingText => "+125% +90%";
    public override string DamageScalingStats => "Melee Damage,Curse";
    public override float AttackCooldown => 1.86f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 10f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Naval , Heavy";
    public override string SpecialEffects => "";
}
