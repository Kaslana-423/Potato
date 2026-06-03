public sealed class SwordTier2GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.sword.tier_2";
    public override string DisplayName => "Sword";
    public override string Description => "Alternates between thrusting and sweeping attacks";
    public override int BasePrice => 51;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override float Damage => 25f;
    public override float AttackCooldown => 1.28f;
    public override float AttackRange => 200f;
    public override string ClassTags => "Blade , Medieval";
    public override string SpecialEffects => "Alternates between thrusting and sweeping attacks";
}
