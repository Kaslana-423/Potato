public sealed class ScrewdriverTier4GeneratedWeapon : ShopWeaponDefinition
{
    public override string Id => "weapon.screwdriver.tier_4";
    public override string DisplayName => "Screwdriver";
    public override string Description => "A landmine spawns every 12s / 9s / 6s / 3s dealing 10(100% ) damage in an area";
    public override int BasePrice => 91;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override float Damage => 20f;
    public override float AttackCooldown => 0.87f;
    public override float AttackRange => 125f;
    public override string ClassTags => "Tool";
    public override string SpecialEffects => "A landmine spawns every 12s / 9s / 6s / 3s dealing 10(100% ) damage in an area";
}
