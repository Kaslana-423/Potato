public sealed class ThunderSwordTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.thunder_sword.tier_4";
    public override string DisplayName => "Thunder Sword";
    public override string Description => "Hitting an enemy spawns 2 / 4 projectiles that slow and damage enemies by 100%";
    public override int BasePrice => 238;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 60f;
    public override float AttackCooldown => 1.06f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Blade , Elemental";
    public override string SpecialEffects => "Hitting an enemy spawns 2 / 4 projectiles that slow and damage enemies by 100%";
}
