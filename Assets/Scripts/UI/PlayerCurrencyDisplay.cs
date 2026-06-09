using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class PlayerCurrencyDisplay : MonoBehaviour
{
    [SerializeField] private PlayerWallet wallet;
    [SerializeField] private bool bindWalletOnEnable = true;
    [SerializeField] private bool autoFindCoinNumTexts = true;
    [SerializeField] private List<TMP_Text> coinTexts = new List<TMP_Text>();

    private bool subscribed;

    private void Awake()
    {
        AutoBindReferences();
    }

    private void Start()
    {
        if (bindWalletOnEnable)
        {
            BindWallet(wallet != null ? wallet : PlayerWallet.GetOrCreate());
        }

        Refresh();
    }

    private void OnEnable()
    {
        AutoBindReferences();
        if (bindWalletOnEnable)
        {
            BindWallet(wallet != null ? wallet : PlayerWallet.GetOrCreate());
        }

        Refresh();
    }

    private void OnDisable()
    {
        UnbindWallet();
    }

    private void Reset()
    {
        AutoBindReferences();
    }

    private void OnValidate()
    {
        AutoBindReferences();
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        RemoveMissingTexts();
        if (!autoFindCoinNumTexts)
        {
            return;
        }

        TMP_Text selfText = GetComponent<TMP_Text>();
        if (selfText != null)
        {
            AddText(selfText);
        }

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            if (text != null && text.name == "CoinNum")
            {
                AddText(text);
            }
        }
    }

    public void BindWallet(PlayerWallet newWallet)
    {
        if (wallet != newWallet)
        {
            UnbindWallet();
            wallet = newWallet;
        }

        if (wallet != null && !subscribed)
        {
            wallet.CoinsChanged += HandleCoinsChanged;
            subscribed = true;
        }

        Refresh();
    }

    public void Refresh()
    {
        if (wallet == null)
        {
            return;
        }

        Refresh(wallet.Coins);
    }

    private void HandleCoinsChanged(PlayerWallet changedWallet, int coins, int delta)
    {
        Refresh(coins);
    }

    private void Refresh(int coins)
    {
        RemoveMissingTexts();
        string valueText = Mathf.Max(0, coins).ToString();
        foreach (TMP_Text text in coinTexts)
        {
            text.text = valueText;
        }
    }

    private void UnbindWallet()
    {
        if (wallet != null && subscribed)
        {
            wallet.CoinsChanged -= HandleCoinsChanged;
        }

        subscribed = false;
    }

    private void AddText(TMP_Text text)
    {
        if (text != null && !coinTexts.Contains(text))
        {
            coinTexts.Add(text);
        }
    }

    private void RemoveMissingTexts()
    {
        for (int index = coinTexts.Count - 1; index >= 0; index--)
        {
            if (coinTexts[index] == null)
            {
                coinTexts.RemoveAt(index);
            }
        }
    }
}
