using System.Collections.Generic;

public sealed class MedikitGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("HP Regeneration", 12f, false),
        new ItemStatModifier("Luck", -10f, false),
    };

    public override string Id => "item.medikit";
    public override string DisplayName => "Medikit";
    public override string Description => "+10 HP Regeneration +2 HP Regeneration every 5 seconds until the end of the wave -10 Luck";
    public override int BasePrice => 95;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
