using System.Collections.Generic;

public sealed class InjectionGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -2f, false),
        new ItemStatModifier("Damage", 7f, true),
    };

    public override string Id => "item.injection";
    public override string DisplayName => "Injection";
    public override string Description => "+7 % Damage -2 Max HP";
    public override int BasePrice => 20;
    public override ShopRarity Rarity => ShopRarity.Tier1;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
