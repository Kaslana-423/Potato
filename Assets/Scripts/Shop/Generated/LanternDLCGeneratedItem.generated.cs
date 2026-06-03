using System.Collections.Generic;

public sealed class LanternDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 10f, true),
        new ItemStatModifier("Range", 50f, false),
        new ItemStatModifier("Knockback", 15f, false),
    };

    public override string Id => "item.lantern_dlc";
    public override string DisplayName => "Lantern (DLC)";
    public override string Description => "+10 % Damage +50 Range +15 Knockback Knocks nearby enemies back every 3 seconds";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
