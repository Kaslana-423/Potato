using System.Collections.Generic;

public sealed class EstySCouchGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", 5f, false),
        new ItemStatModifier("HP Regeneration", 2f, false),
        new ItemStatModifier("Speed", -21f, true),
    };

    public override string Id => "item.esty_s_couch";
    public override string DisplayName => "Esty's Couch";
    public override string Description => "+5 Max HP +2 HP Regeneration for every permanent -1 % Speed you have -20 % Speed";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
