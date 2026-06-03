using System.Collections.Generic;

public sealed class SeashellDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -10f, true),
    };

    public override string Id => "item.seashell_dlc";
    public override string DisplayName => "Seashell (DLC)";
    public override string Description => "Every ranged weapon's 5th projectile has +3 projectiles -10 % Damage";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
