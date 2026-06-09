using System;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour
{
    public delegate void PlayerHealthChangedHandler(PlayerHealth health, int currentHealth, int maxHealth, int delta);

    [SerializeField, Min(1)] private int fallbackMaxHealth = 10;
    [SerializeField, Min(0f)] private float invulnerabilitySeconds = 0.35f;

    private PlayerStats playerStats;
    public int currentHealth;
    private int cachedMaxHealth;
    private float nextDamageTime;
    private bool initialized;

    public event Action<PlayerHealth> Died;
    public event Action<PlayerHealth, int> Damaged;
    public event Action<PlayerHealth, int> Healed;
    public event PlayerHealthChangedHandler HealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => playerStats != null ? Mathf.Max(1, playerStats.MaxHp) : fallbackMaxHealth;
    public float HealthPercent => MaxHealth > 0 ? Mathf.Clamp01((float)currentHealth / MaxHealth) : 0f;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        AcquirePlayerStats();
        cachedMaxHealth = MaxHealth;
        currentHealth = cachedMaxHealth;
        initialized = true;
    }

    private void OnEnable()
    {
        AcquirePlayerStats();
        if (playerStats != null)
        {
            playerStats.StatsChanged += HandleStatsChanged;
        }

        if (!initialized)
        {
            currentHealth = MaxHealth;
            cachedMaxHealth = MaxHealth;
            initialized = true;
        }
        else
        {
            RefreshMaxHealth(false);
        }
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged -= HandleStatsChanged;
        }
    }

    private void OnValidate()
    {
        fallbackMaxHealth = Mathf.Max(1, fallbackMaxHealth);
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

        int finalDamage = CalculateIncomingDamage(rawDamage);
        if (finalDamage <= 0)
        {
            return false;
        }

        int oldHealth = currentHealth;
        SetCurrentHealthInternal(currentHealth - finalDamage, true);
        int actualDamage = oldHealth - currentHealth;

        if (actualDamage <= 0)
        {
            return false;
        }

        nextDamageTime = Time.time + invulnerabilitySeconds;
        Damaged?.Invoke(this, actualDamage);

        if (currentHealth <= 0)
        {
            IsDead = true;
            Died?.Invoke(this);
        }

        return true;
    }

    public void Heal(float amount)
    {
        Heal(Mathf.RoundToInt(Mathf.Max(0f, amount)));
    }

    public void Heal(int amount)
    {
        if (IsDead)
        {
            return;
        }

        int oldHealth = currentHealth;
        SetCurrentHealthInternal(currentHealth + Mathf.Max(0, amount), true);
        int healedAmount = currentHealth - oldHealth;
        if (healedAmount > 0)
        {
            Healed?.Invoke(this, healedAmount);
        }
    }

    public void SetCurrentHealth(int newCurrentHealth)
    {
        SetCurrentHealthInternal(newCurrentHealth, true);
    }

    public void Refill()
    {
        IsDead = false;
        SetCurrentHealthInternal(MaxHealth, true);
    }

    private int CalculateIncomingDamage(float rawDamage)
    {
        float damage = Mathf.Max(0f, rawDamage);
        if (playerStats != null)
        {
            damage = Mathf.Max(0f, damage - Mathf.Max(0, playerStats.Armor));
        }

        if (damage <= 0f)
        {
            return 0;
        }

        return Mathf.Max(1, Mathf.RoundToInt(damage));
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
        RefreshMaxHealth(true);
    }

    private void AcquirePlayerStats()
    {
        if (playerStats != null)
        {
            return;
        }

        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            playerStats = PlayerStats.Instance;
        }
    }

    private void RefreshMaxHealth(bool notify)
    {
        int oldMaxHealth = cachedMaxHealth;
        int oldHealth = currentHealth;

        cachedMaxHealth = MaxHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, cachedMaxHealth);

        if (notify && (oldMaxHealth != cachedMaxHealth || oldHealth != currentHealth))
        {
            NotifyHealthChanged(currentHealth - oldHealth);
        }
    }

    private void SetCurrentHealthInternal(int newCurrentHealth, bool notify)
    {
        int oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(newCurrentHealth, 0, MaxHealth);

        if (notify && oldHealth != currentHealth)
        {
            NotifyHealthChanged(currentHealth - oldHealth);
        }
    }

    private void NotifyHealthChanged(int delta)
    {
        HealthChanged?.Invoke(this, currentHealth, MaxHealth, delta);
    }
}
