using UnityEngine;

public sealed class LootCratePickup : MagneticBattlefieldDrop
{
    protected override bool ApplyPickup(Transform player)
    {
        PlayerLootCrateInventory inventory = PlayerLootCrateInventory.GetOrCreate();
        if (inventory == null)
        {
            return false;
        }

        inventory.AddCrates(1);
        return true;
    }
}
