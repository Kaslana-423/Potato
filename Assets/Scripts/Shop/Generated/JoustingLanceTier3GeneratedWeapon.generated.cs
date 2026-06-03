public sealed class JoustingLanceTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.jousting_lance.tier_3";
    public override string DisplayName => "Jousting Lance";
    public override string Description => "+2 / +3 / +4 / +5 % Speed -10 / -15 / -20 / -25 % Damage while standing still";
    public override int BasePrice => 72;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 30f;
    public override float AttackCooldown => 1.42f;
    public override float AttackRange => 250f;
    public override string ClassTags => "Medieval";
    public override string SpecialEffects => "+2 / +3 / +4 / +5 % Speed -10 / -15 / -20 / -25 % Damage while standing still";
}
