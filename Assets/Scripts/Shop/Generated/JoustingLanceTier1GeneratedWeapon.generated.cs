public sealed class JoustingLanceTier1GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.jousting_lance.tier_1";
    public override string DisplayName => "Jousting Lance";
    public override string Description => "+2 / +3 / +4 / +5 % Speed -10 / -15 / -20 / -25 % Damage while standing still";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override float Damage => 20f;
    public override float AttackCooldown => 1.58f;
    public override float AttackRange => 250f;
    public override string ClassTags => "Medieval";
    public override string SpecialEffects => "+2 / +3 / +4 / +5 % Speed -10 / -15 / -20 / -25 % Damage while standing still";
}
