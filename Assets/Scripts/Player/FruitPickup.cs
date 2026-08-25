using UnityEngine;

public sealed class FruitPickup : MagneticBattlefieldDrop
{
    [SerializeField, Min(1)] private int baseHealing = 3;

    protected override bool CanStartCollecting(Transform player)
    {
        PlayerHealth health = ResolveHealth(player);
        return health != null && !health.IsDead && health.CurrentHealth < health.MaxHealth;
    }

    protected override bool ApplyPickup(Transform player)
    {
        PlayerHealth health = ResolveHealth(player);
        if (health == null || health.IsDead || health.CurrentHealth >= health.MaxHealth)
        {
            return false;
        }

        int healingBonus = PlayerStats.Instance != null ? PlayerStats.Instance.ConsumableHeal : 0;
        health.Heal(Mathf.Max(1, baseHealing + healingBonus));
        return true;
    }

    private static PlayerHealth ResolveHealth(Transform player)
    {
        PlayerHealth health = player != null ? player.GetComponentInParent<PlayerHealth>() : null;
        if (health == null && PlayerStats.Instance != null)
        {
            health = PlayerStats.Instance.GetComponent<PlayerHealth>();
        }

        return health;
    }
}
