public sealed class ClawWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.claw.tier_1";
    public override string DisplayName => "Claw";
    public override string Description => "A precise unarmed weapon with a short cooldown.";
    public override int BasePrice => 15;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.Slash;
    public override float Damage => 5f;
    public override float AttackCooldown => 0.78f;
    public override float AttackRange => 150f;
    public override string ClassTags => "Unarmed, Precise";
}
