public sealed class KnifeWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.knife.tier_1";
    public override string DisplayName => "Knife";
    public override string Description => "A compact precise weapon for quick melee attacks.";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Thrust;
    public override float Damage => 6f;
    public override float AttackCooldown => 1.01f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Precise";
}
