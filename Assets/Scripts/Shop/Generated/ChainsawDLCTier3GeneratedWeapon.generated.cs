public sealed class ChainsawDLCTier3GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.chainsaw_dlc.tier_3";
    public override string DisplayName => "Chainsaw (DLC)";
    public override string Description => "Cooldown is 1.83s / 1.52s every 30 / 40 shots Deals 10% / 20% of an enemy’s current health as bonus damage ( 1% / 2% for bosses and elites)";
    public override int BasePrice => 112;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override float Damage => 10f;
    public override float AttackCooldown => 0.53f;
    public override float AttackRange => 175f;
    public override string ClassTags => "Blade , Tool";
    public override string SpecialEffects => "Cooldown is 1.83s / 1.52s every 30 / 40 shots Deals 10% / 20% of an enemy’s current health as bonus damage ( 1% / 2% for bosses and elites)";
}
