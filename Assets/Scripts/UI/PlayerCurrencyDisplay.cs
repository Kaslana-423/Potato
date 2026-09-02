using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class PlayerCurrencyDisplay : MonoBehaviour
{
    [SerializeField] private PlayerWallet wallet;
    [SerializeField] private bool bindWalletOnEnable = true;
    [SerializeField] private bool autoFindCoinNumTexts = true;
    [SerializeField] private List<TMP_Text> coinTexts = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> retainedMaterialTexts = new List<TMP_Text>();

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
            else if (text != null && text.name == "StoredMaterialNum")
            {
                AddRetainedMaterialText(text);
            }
        }

        if (Application.isPlaying)
        {
            EnsureRetainedMaterialUi();
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
            wallet.RetainedMaterialsChanged += HandleRetainedMaterialsChanged;
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

        RefreshCoins(wallet.Coins);
        RefreshRetainedMaterials(wallet.RetainedMaterials);
    }

    private void HandleCoinsChanged(PlayerWallet changedWallet, int coins, int delta)
    {
        RefreshCoins(coins);
    }

    private void HandleRetainedMaterialsChanged(PlayerWallet changedWallet, int materials, int delta)
    {
        RefreshRetainedMaterials(materials);
    }

    private void RefreshCoins(int coins)
    {
        RemoveMissingTexts();
        string valueText = Mathf.Max(0, coins).ToString();
        foreach (TMP_Text text in coinTexts)
        {
            text.text = valueText;
        }
    }

    private void RefreshRetainedMaterials(int materials)
    {
        RemoveMissingTexts();
        string valueText = $"储存 {Mathf.Max(0, materials)}";
        foreach (TMP_Text text in retainedMaterialTexts)
        {
            text.text = valueText;
        }
    }

    private void UnbindWallet()
    {
        if (wallet != null && subscribed)
        {
            wallet.CoinsChanged -= HandleCoinsChanged;
            wallet.RetainedMaterialsChanged -= HandleRetainedMaterialsChanged;
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

    private void AddRetainedMaterialText(TMP_Text text)
    {
        if (text != null && !retainedMaterialTexts.Contains(text))
        {
            retainedMaterialTexts.Add(text);
        }
    }

    private void EnsureRetainedMaterialUi()
    {
        if (GetComponent<PlayerHealthBarView>() == null || retainedMaterialTexts.Count > 0)
        {
            return;
        }

        Transform existing = transform.Find("StoredMaterials");
        if (existing != null)
        {
            TMP_Text existingText = existing.Find("StoredMaterialNum")?.GetComponent<TMP_Text>();
            AddRetainedMaterialText(existingText);
            return;
        }

        RectTransform coinRect = transform.Find("Coin") as RectTransform;
        if (coinRect == null)
        {
            return;
        }

        GameObject storedMaterials = Instantiate(coinRect.gameObject, transform);
        storedMaterials.name = "StoredMaterials";
        RectTransform storedRect = storedMaterials.GetComponent<RectTransform>();
        TMP_Text storedText = storedMaterials.GetComponentInChildren<TMP_Text>(true);
        float textWidth = storedText != null ? storedText.rectTransform.sizeDelta.x : 240f;
        storedRect.anchoredPosition = coinRect.anchoredPosition
            + Vector2.right * (coinRect.sizeDelta.x + textWidth + 30f);

        if (storedText != null)
        {
            storedText.name = "StoredMaterialNum";
            storedText.enableAutoSizing = true;
            storedText.fontSizeMin = 20f;
            storedText.fontSizeMax = 48f;
            storedText.text = "储存 0";
            AddRetainedMaterialText(storedText);
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

        for (int index = retainedMaterialTexts.Count - 1; index >= 0; index--)
        {
            if (retainedMaterialTexts[index] == null)
            {
                retainedMaterialTexts.RemoveAt(index);
            }
        }
    }
}
