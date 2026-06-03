public sealed class PlasmaSledgeTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.plasma_sledge.tier_3";
    public override string DisplayName => "Plasma Sledge";
    public override string Description => "Hits have 25% / 50% chance to explode.";
    public override int BasePrice => 136;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 80f;
    public override float AttackCooldown => 1.55f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Elemental , Explosive";
    public override string SpecialEffects => "Hits have 25% / 50% chance to explode.";
}
