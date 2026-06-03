using System.Collections.Generic;

public sealed class BloodDonationGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Harvesting", 40f, false),
    };

    public override string Id => "item.blood_donation";
    public override string DisplayName => "Blood Donation";
    public override string Description => "+40 Harvesting You take 1 damage per second (does not give invulnerability time)";
    public override int BasePrice => 50;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
