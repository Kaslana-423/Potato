using UnityEngine;

public sealed class PlayerLootCrateInventory : MonoBehaviour
{
    [SerializeField, Min(0)] private int pendingCrates;

    public int PendingCrates => pendingCrates;

    public static PlayerLootCrateInventory GetOrCreate()
    {
        PlayerLootCrateInventory existing = FindObjectOfType<PlayerLootCrateInventory>(true);
        if (existing != null)
        {
            return existing;
        }

        GameObject owner = PlayerStats.Instance != null ? PlayerStats.Instance.gameObject : null;
        if (owner == null)
        {
            owner = GameObject.FindGameObjectWithTag("Player");
        }

        return owner != null ? owner.AddComponent<PlayerLootCrateInventory>() : null;
    }

    public void AddCrates(int amount)
    {
        pendingCrates += Mathf.Max(0, amount);
    }

    public bool TryConsumeCrate()
    {
        if (pendingCrates <= 0)
        {
            return false;
        }

        pendingCrates--;
        return true;
    }
}
