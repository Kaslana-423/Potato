public sealed class ChopperWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.chopper.tier_1";
    public override string DisplayName => "Chopper";
    public override string Description => "A fast blade for close-range attacks.";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 6f;
    public override float AttackCooldown => 0.99f;
    public override float AttackRange => 135f;
    public override string ClassTags => "Blade";
}
