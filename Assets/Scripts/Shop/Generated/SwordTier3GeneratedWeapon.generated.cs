public sealed class SwordTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sword.tier_3";
    public override string DisplayName => "Sword";
    public override string Description => "Alternates between thrusting and sweeping attacks";
    public override int BasePrice => 95;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 40f;
    public override float AttackCooldown => 1.13f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Blade , Medieval";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks";
}
