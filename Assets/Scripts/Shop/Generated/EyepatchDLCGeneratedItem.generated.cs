using System.Collections.Generic;

public sealed class EyepatchDLCGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Crit Chance", 3f, true),
    };

    public override string Id => "item.eyepatch_dlc";
    public override string DisplayName => "Eyepatch (DLC)";
    public override string Description => "Projectiles get +1 piercing on critical hit +3 % Crit Chance -10% Accuracy";
    public override int BasePrice => 55;
    public override ShopRarity Rarity => ShopRarity.Tier2;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
