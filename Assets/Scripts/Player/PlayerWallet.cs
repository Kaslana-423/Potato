using System;
using UnityEngine;

public sealed class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    public event Action<PlayerWallet, int, int> CoinsChanged;

    [Header("Currency")]
    [SerializeField, Min(0)] private int startingCoins = 0;
    [SerializeField, Min(0)] private int coins = 0;
    [SerializeField] private bool resetToStartingCoinsOnAwake = true;

    public int Coins => coins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Scene already has a PlayerWallet singleton. Disabling duplicate component.", this);
            enabled = false;
            return;
        }

        Instance = this;
        if (resetToStartingCoinsOnAwake)
        {
            coins = Mathf.Max(0, startingCoins);
        }
    }

    private void Start()
    {
        NotifyCoinsChanged(0);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnValidate()
    {
        startingCoins = Mathf.Max(0, startingCoins);
        coins = Mathf.Max(0, coins);
    }

    public static PlayerWallet GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        PlayerWallet existingWallet = FindObjectOfType<PlayerWallet>(true);
        if (existingWallet != null)
        {
            return existingWallet;
        }

        if (PlayerStats.Instance != null)
        {
            return PlayerStats.Instance.gameObject.AddComponent<PlayerWallet>();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return player.AddComponent<PlayerWallet>();
        }

        GameObject walletObject = new GameObject("PlayerWallet");
        return walletObject.AddComponent<PlayerWallet>();
    }

    public void SetCoins(int amount)
    {
        int clampedAmount = Mathf.Max(0, amount);
        if (coins == clampedAmount)
        {
            return;
        }

        int oldCoins = coins;
        coins = clampedAmount;
        NotifyCoinsChanged(coins - oldCoins);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        SetCoins(coins + amount);
    }

    public bool CanSpend(int amount)
    {
        return amount <= 0 || coins >= amount;
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (!CanSpend(amount))
        {
            return false;
        }

        SetCoins(coins - amount);
        return true;
    }

    public void NotifyCoinsChanged(int delta = 0)
    {
        CoinsChanged?.Invoke(this, coins, delta);
    }
}
