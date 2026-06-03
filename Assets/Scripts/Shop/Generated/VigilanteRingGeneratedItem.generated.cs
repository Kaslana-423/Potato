using System.Collections.Generic;

public sealed class VigilanteRingGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 3f, true),
    };

    public override string Id => "item.vigilante_ring";
    public override string DisplayName => "Vigilante Ring";
    public override string Description => "+3 % Damage at the end of a wave";
    public override int BasePrice => 92;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
