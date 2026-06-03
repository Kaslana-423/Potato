public sealed class CactiClubWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.cacti_club.tier_1";
    public override string DisplayName => "Cacti Club";
    public override string Description => "A heavy primitive club that launches projectiles after a hit.";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 10f;
    public override float AttackCooldown => 1.66f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Primitive, Heavy";
    public override string SpecialEffects => "Hitting an enemy spawns 3 projectiles dealing 50% damage.";
}
