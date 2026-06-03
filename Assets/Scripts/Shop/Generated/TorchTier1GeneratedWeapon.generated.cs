public sealed class TorchTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.torch.tier_1";
    public override string DisplayName => "Torch";
    public override string Description => "Deals 3x3 / 5x5 / 8x6 / 12x9 (100% ) burning damage. Burning spreads to 0 / 0 / 1 / 1 additional nearby enemies.";
    public override int BasePrice => 10;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override float Damage => 1f;
    public override float AttackCooldown => 1.1f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Primitive , Elemental";
    public override string SpecialEffects => "Deals 3x3 / 5x5 / 8x6 / 12x9 (100% ) burning damage. Burning spreads to 0 / 0 / 1 / 1 additional nearby enemies.";
}
