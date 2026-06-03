using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopManager : MonoBehaviour
{
    [Header("Offers")]
    [SerializeField, Min(1)] private int offerCount = 4;
    [SerializeField] private ShopOfferView shopItemPrefab = null;
    [SerializeField] private Transform shopItemContainer;

    [Header("Optional UI References")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Text statusText;

    [Header("Bags")]
    [SerializeField] private RelicBag relicBag;
    [SerializeField] private WeaponBag weaponBag;

    [Header("Prototype")]
    [SerializeField] private bool buildPrototypeUiWhenViewsMissing = true;
    [SerializeField] private bool refreshOnStart = true;

    private ShopOfferView[] offerViews = Array.Empty<ShopOfferView>();
    private readonly List<ShopContentDefinition> currentOffers = new List<ShopContentDefinition>();
    private Button boundRefreshButton;
    private bool prototypeUiBuilt;

    public IReadOnlyList<ShopContentDefinition> CurrentOffers => currentOffers;

    private void Awake()
    {
        AutoBindReferences();
    }

    private void Start()
    {
        EnsureUi();
        if (refreshOnStart)
        {
            RefreshShop();
        }
    }

    public void ConfigureUi(ShopOfferView[] views, Button newRefreshButton, TMP_Text newStatusText)
    {
        offerViews = views ?? Array.Empty<ShopOfferView>();
        refreshButton = newRefreshButton;
        statusText = newStatusText;
        BindRefreshButton();
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        if (shopItemContainer == null)
        {
            shopItemContainer = FindDescendant("ShopItemContainer");
        }

        if (refreshButton == null)
        {
            refreshButton = FindComponent<Button>("RefreshButton", "Refresh Button");
        }

        if (statusText == null)
        {
            statusText = FindComponent<TMP_Text>("StatusText", "Status");
        }

        if (relicBag == null)
        {
            relicBag = FindOrAddComponent<RelicBag>("RelicBag");
        }

        if (weaponBag == null)
        {
            weaponBag = FindOrAddComponent<WeaponBag>("WeaponBag");
        }
    }

    public void RefreshShop()
    {
        EnsureUi();

        currentOffers.Clear();
        var pool = new List<ShopContentDefinition>(ShopContentCatalog.All);
        Shuffle(pool);

        int count = Mathf.Min(offerCount, pool.Count);
        for (int index = 0; index < count; index++)
        {
            currentOffers.Add(pool[index]);
        }

        for (int index = 0; index < offerViews.Length; index++)
        {
            ShopOfferView view = offerViews[index];
            if (view == null)
            {
                continue;
            }

            if (index < currentOffers.Count)
            {
                view.Bind(currentOffers[index], SelectOffer, TryPurchaseOffer);
                view.SetVisible(true);
            }
            else
            {
                view.SetVisible(false);
            }
        }

        SetStatus($"已刷新 {currentOffers.Count} 个商品。点击卡片查看详情。");
    }

    private void EnsureUi()
    {
        AutoBindReferences();
        EnsureOfferViews();

        if (!HasOfferViews() && buildPrototypeUiWhenViewsMissing && !prototypeUiBuilt)
        {
            prototypeUiBuilt = true;
            ShopPrototypeUiFactory.Build(this);
        }

        BindRefreshButton();
    }

    private void EnsureOfferViews()
    {
        var views = offerViews == null
            ? new List<ShopOfferView>()
            : offerViews.Where(view => view != null).ToList();

        if (shopItemPrefab != null)
        {
            Transform parent = shopItemContainer != null ? shopItemContainer : transform;
            while (views.Count < offerCount)
            {
                ShopOfferView view = Instantiate(shopItemPrefab, parent);
                view.name = $"{shopItemPrefab.name} {views.Count + 1}";
                views.Add(view);
            }
        }

        offerViews = views.ToArray();
    }

    private bool HasOfferViews()
    {
        return offerViews != null && offerViews.Length > 0;
    }

    private void BindRefreshButton()
    {
        if (boundRefreshButton == refreshButton)
        {
            return;
        }

        if (boundRefreshButton != null)
        {
            boundRefreshButton.onClick.RemoveListener(RefreshShop);
        }

        boundRefreshButton = refreshButton;
        if (boundRefreshButton != null)
        {
            boundRefreshButton.onClick.AddListener(RefreshShop);
        }
    }

    private void SelectOffer(ShopContentDefinition content)
    {
        if (content == null)
        {
            return;
        }

        SetStatus($"{content.LocalizedDisplayName}（{content.RarityLabel}）\n{content.BuildDetails()}");
    }

    private void TryPurchaseOffer(ShopOfferView offerView, ShopContentDefinition content)
    {
        if (content == null)
        {
            return;
        }

        bool purchased;
        string failureReason;
        if (content.Kind == ShopContentKind.Weapon)
        {
            if (weaponBag == null)
            {
                SetStatus("没有绑定 WeaponBag，无法购买武器。");
                return;
            }

            purchased = weaponBag.TryAdd(content, out failureReason);
        }
        else
        {
            if (relicBag == null)
            {
                SetStatus("没有绑定 RelicBag，无法购买道具。");
                return;
            }

            purchased = relicBag.TryAdd(content, out failureReason);
        }

        if (purchased)
        {
            if (offerView != null)
            {
                offerView.MarkPurchased();
            }

            SetStatus($"已购买 {content.LocalizedDisplayName}，放入{(content.Kind == ShopContentKind.Weapon ? "武器背包" : "道具背包")}。");
        }
        else
        {
            SetStatus(failureReason);
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private static void Shuffle(IList<ShopContentDefinition> contents)
    {
        for (int index = contents.Count - 1; index > 0; index--)
        {
            int swapIndex = UnityEngine.Random.Range(0, index + 1);
            (contents[index], contents[swapIndex]) = (contents[swapIndex], contents[index]);
        }
    }

    private Transform FindDescendant(params string[] names)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (child.name == objectName)
                {
                    return child;
                }
            }
        }

        return null;
    }

    private T FindComponent<T>(params string[] names) where T : Component
    {
        Transform child = FindDescendant(names);
        return child != null ? child.GetComponent<T>() : null;
    }

    private T FindOrAddComponent<T>(params string[] names) where T : Component
    {
        T existing = FindComponent<T>(names);
        if (existing != null)
        {
            return existing;
        }

        Transform child = FindDescendant(names);
        return child != null ? child.gameObject.AddComponent<T>() : null;
    }
}
