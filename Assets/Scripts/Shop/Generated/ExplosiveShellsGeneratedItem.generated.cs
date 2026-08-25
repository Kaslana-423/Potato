using System.Collections.Generic;

public sealed class ExplosiveShellsGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", -15f, true),
        new ItemStatModifier("Explosion Damage", 60f, true),
        new ItemStatModifier("Explosion Size", 15f, true),
    };

    public override string Id => "item.explosive_shells";
    public override string DisplayName => "Explosive Shells";
    public override string Description => "+60 % Explosion Damage +15 % Explosion Size -15 % Damage";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
