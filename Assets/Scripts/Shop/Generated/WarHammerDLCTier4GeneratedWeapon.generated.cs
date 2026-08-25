public sealed class WarHammerDLCTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.war_hammer_dlc.tier_4";
    public override string DisplayName => "War Hammer (DLC)";
    public override string Description => "Resets the cooldown of all offensive turrets when attacking";
    public override int BasePrice => 255;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 180f;
    public override string DamageScalingText => "+200% +150%";
    public override string DamageScalingStats => "Melee Damage,Engineering";
    public override float AttackCooldown => 1.95f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 25f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blunt , Heavy";
    public override string SpecialEffects => "Resets the cooldown of all offensive turrets when attacking";
}
