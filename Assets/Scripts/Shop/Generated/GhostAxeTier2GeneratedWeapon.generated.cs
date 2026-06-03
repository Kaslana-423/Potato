public sealed class GhostAxeTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.ghost_axe.tier_2";
    public override string DisplayName => "Ghost Axe";
    public override string Description => "+1% Damage for every 20 / 18 / 16 / 12 kills in a wave with this weapon";
    public override int BasePrice => 39;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 18f;
    public override float AttackCooldown => 1.66f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Ethereal";
    public override string SpecialEffects => "+1% Damage for every 20 / 18 / 16 / 12 kills in a wave with this weapon";
}
