using System.Collections.Generic;

public sealed class BallAndChainGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Damage", 15f, true),
        new ItemStatModifier("Armor", 3f, false),
        new ItemStatModifier("Speed", -3f, true),
        new ItemStatModifier("Knockback", 5f, false),
    };

    public override string Id => "item.ball_and_chain";
    public override string DisplayName => "Ball and Chain";
    public override string Description => "+15 % Damage +3 Armor +5 Knockback -3 % Speed Weapons have a minimum cooldown of 0.75 seconds between attacks";
    public override int BasePrice => 75;
    public override ShopRarity Rarity => ShopRarity.Tier3;
    public override int PurchaseLimit => 1;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
