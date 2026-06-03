using System.Collections.Generic;

public sealed class RobotArmGeneratedItem : ShopItemDefinition
{
    private static readonly ItemStatModifier[] modifiers =
    {
        new ItemStatModifier("Max HP", -1f, false),
        new ItemStatModifier("Melee Damage", 3f, false),
        new ItemStatModifier("Engineering", 3f, false),
    };

    public override string Id => "item.robot_arm";
    public override string DisplayName => "Robot Arm";
    public override string Description => "+3 Melee Damage at the end of a wave +3 Engineering at the end of a wave -1 Max HP at the end of a wave";
    public override int BasePrice => 100;
    public override ShopRarity Rarity => ShopRarity.Tier4;
    public override int PurchaseLimit => 0;
    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;
}
