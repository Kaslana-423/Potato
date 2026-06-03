public sealed class CaptainSSwordDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.captain_s_sword_dlc.tier_3";
    public override string DisplayName => "Captain's Sword (DLC)";
    public override string Description => "Alternates between thrusting and sweeping attacks Deals +25 / +50 Damage for every free weapon slot you have.";
    public override int BasePrice => 110;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 50f;
    public override float AttackCooldown => 1.03f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Naval , Blade";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks Deals +25 / +50 Damage for every free weapon slot you have.";
}
