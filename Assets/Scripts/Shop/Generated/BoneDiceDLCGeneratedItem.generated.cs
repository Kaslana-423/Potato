using System.Collections.Generic;

public sealed class BoneDiceDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -1f, false),
        new ItemStatModifier("Damage", 1f, true),
    };

    public override string Id => "item.bone_dice_dlc";
    public override string DisplayName => "Bone Dice (DLC)";
    public override string Description => "+50% chance to get +1 % Damage when rerolling in the shop +10% chance to get -1 Max HP when rerolling in the shop";
    public override int BasePrice => 30;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 2;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
