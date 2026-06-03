public sealed class SwordTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sword.tier_4";
    public override string DisplayName => "Sword";
    public override string Description => "Alternates between thrusting and sweeping attacks";
    public override int BasePrice => 190;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 60f;
    public override float AttackCooldown => 0.98f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Blade , Medieval";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks";
}
