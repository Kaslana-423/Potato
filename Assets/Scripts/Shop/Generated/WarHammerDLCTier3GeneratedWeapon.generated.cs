public sealed class WarHammerDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.war_hammer_dlc.tier_3";
    public override string DisplayName => "War Hammer (DLC)";
    public override string Description => "Resets the cooldown of all offensive turrets when attacking";
    public override int BasePrice => 130;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 100f;
    public override string DamageScalingText => "+150% +100%";
    public override string DamageScalingStats => "Melee Damage,Engineering";
    public override float AttackCooldown => 2.11f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 1.5f;
    public override float CritChance => 3f;
    public override float Knockback => 20f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Blunt , Heavy";
    public override string SpecialEffects => "Resets the cooldown of all offensive turrets when attacking";
}
