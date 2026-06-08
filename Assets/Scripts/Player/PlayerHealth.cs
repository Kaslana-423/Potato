using System;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float fallbackMaxHealth = 10f;
    [SerializeField, Min(0f)] private float invulnerabilitySeconds = 0.35f;

    private PlayerStats playerStats;
    private float currentHealth;
    private float nextDamageTime;
    private bool initialized;

    public event Action<PlayerHealth> Died;
    public event Action<PlayerHealth, float> Damaged;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => playerStats != null ? Mathf.Max(1f, playerStats.MaxHp) : fallbackMaxHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            playerStats = PlayerStats.Instance;
        }

        currentHealth = MaxHealth;
        initialized = true;
    }

    private void OnEnable()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged += HandleStatsChanged;
        }

        if (!initialized)
        {
            currentHealth = MaxHealth;
            initialized = true;
        }
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged -= HandleStatsChanged;
        }
    }

    public bool TakeDamage(float rawDamage)
    {
        if (IsDead || Time.time < nextDamageTime)
        {
            return false;
        }

        if (TryDodge())
        {
            nextDamageTime = Time.time + invulnerabilitySeconds;
            return false;
        }

        float finalDamage = CalculateIncomingDamage(rawDamage);
        if (finalDamage <= 0f)
        {
            return false;
        }

        currentHealth = Mathf.Max(0f, currentHealth - finalDamage);
        nextDamageTime = Time.time + invulnerabilitySeconds;
        Damaged?.Invoke(this, finalDamage);

        if (currentHealth <= 0f)
        {
            IsDead = true;
            Died?.Invoke(this);
        }

        return true;
    }

    public void Heal(float amount)
    {
        if (IsDead)
        {
            return;
        }

        currentHealth = Mathf.Min(MaxHealth, currentHealth + Mathf.Max(0f, amount));
    }

    private float CalculateIncomingDamage(float rawDamage)
    {
        float damage = Mathf.Max(0f, rawDamage);
        if (playerStats != null)
        {
            damage = Mathf.Max(1f, damage - Mathf.Max(0f, playerStats.Armor));
        }

        return damage;
    }

    private bool TryDodge()
    {
        if (playerStats == null || playerStats.Dodge <= 0)
        {
            return false;
        }

        float dodgeChance = Mathf.Clamp01(playerStats.Dodge / 100f);
        return UnityEngine.Random.value < dodgeChance;
    }

    private void HandleStatsChanged(PlayerStats stats)
    {
        currentHealth = Mathf.Min(currentHealth, MaxHealth);
    }
}
