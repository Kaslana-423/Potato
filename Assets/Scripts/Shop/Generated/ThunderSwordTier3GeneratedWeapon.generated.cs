public sealed class ThunderSwordTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.thunder_sword.tier_3";
    public override string DisplayName => "Thunder Sword";
    public override string Description => "Hitting an enemy spawns 2 / 4 projectiles that slow and damage enemies by 100%";
    public override int BasePrice => 119;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 30f;
    public override float AttackCooldown => 1.21f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Blade , Elemental";
    public override string SpecialEffects => "Hitting an enemy spawns 2 / 4 projectiles that slow and damage enemies by 100%";
}
