public sealed class CaptainSSwordDLCTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.captain_s_sword_dlc.tier_4";
    public override string DisplayName => "Captain's Sword (DLC)";
    public override string Description => "Alternates between thrusting and sweeping attacks Deals +25 / +50 Damage for every free weapon slot you have.";
    public override int BasePrice => 210;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 80f;
    public override string DamageScalingText => "+125% +125%";
    public override string DamageScalingStats => "Melee Damage,Curse";
    public override float AttackCooldown => 0.95f;
    public override float AttackRange => 200f;
    public override float CritMultiplier => 2f;
    public override float CritChance => 3f;
    public override float Knockback => 5f;
    public override float LifeSteal => 0f;
    public override string ClassTags => "Naval , Blade";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks Deals +25 / +50 Damage for every free weapon slot you have.";
}
