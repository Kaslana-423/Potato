using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public sealed class ShopRarityWeightProfile
{
    [SerializeField, Min(1)] private int wave = 1;
    [SerializeField, Min(0f)] private float tier1Weight = 90f;
    [SerializeField, Min(0f)] private float tier2Weight = 10f;
    [SerializeField, Min(0f)] private float tier3Weight;
    [SerializeField, Min(0f)] private float tier4Weight;

    public int Wave => wave;

    public ShopRarityWeightProfile()
    {
    }

    public ShopRarityWeightProfile(
        int wave,
        float tier1Weight,
        float tier2Weight,
        float tier3Weight,
        float tier4Weight)
    {
        this.wave = wave;
        this.tier1Weight = tier1Weight;
        this.tier2Weight = tier2Weight;
        this.tier3Weight = tier3Weight;
        this.tier4Weight = tier4Weight;
    }

    public float GetWeight(ShopRarity rarity)
    {
        switch (rarity)
        {
            case ShopRarity.Tier1:
                return tier1Weight;
            case ShopRarity.Tier2:
                return tier2Weight;
            case ShopRarity.Tier3:
                return tier3Weight;
            case ShopRarity.Tier4:
                return tier4Weight;
            default:
                return 0f;
        }
    }

    public void ClampValues()
    {
        wave = Mathf.Max(1, wave);
        tier1Weight = Mathf.Max(0f, tier1Weight);
        tier2Weight = Mathf.Max(0f, tier2Weight);
        tier3Weight = Mathf.Max(0f, tier3Weight);
        tier4Weight = Mathf.Max(0f, tier4Weight);
    }
}

public sealed class ShopManager : MonoBehaviour
{
    private static readonly Color AffordableButtonColor = new Color(0.22f, 0.62f, 0.24f, 1f);
    private static readonly Color UnaffordableButtonColor = new Color(0.28f, 0.28f, 0.28f, 0.82f);

    [Header("Window")]
    [SerializeField] private GameObject shopWindowRoot;
    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private bool startOpen = false;
    [SerializeField] private bool refreshWhenOpenedIfEmpty = true;

    [Header("Offers")]
    [SerializeField, Min(1)] private int offerCount = 4;
    [SerializeField] private ShopOfferView shopItemPrefab = null;
    [SerializeField] private Transform shopItemContainer;

    [Header("Optional UI References")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Text refreshLabelText;
    [SerializeField] private TMP_Text refreshCostText;
    [SerializeField] private TMP_Text statusText;

    [Header("Bags")]
    [SerializeField] private RelicBag relicBag;
    [SerializeField] private WeaponBag weaponBag;
    [SerializeField] private PlayerWeaponEquipment playerWeaponEquipment;

    [Header("Currency")]
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private PlayerCurrencyDisplay currencyDisplay;

    [Header("Refresh")]
    [SerializeField] private bool refreshOnStart = true;
    [SerializeField, Min(0)] private int baseRefreshCost = 1;
    [SerializeField, Min(0)] private int refreshCostIncrease = 1;

    [Header("Rarity Progression")]
    [SerializeField] private EnemySpawner waveSource;
    [SerializeField] private List<ShopRarityWeightProfile> rarityWeightProfiles =
        new List<ShopRarityWeightProfile>
        {
            new ShopRarityWeightProfile(1, 90f, 10f, 0f, 0f),
            new ShopRarityWeightProfile(5, 60f, 28f, 10f, 0f),
            new ShopRarityWeightProfile(10, 40f, 35f, 20f, 5f)
        };
    [SerializeField, Min(1)] private int tier2UnlockWave = 1;
    [SerializeField, Min(1)] private int tier3UnlockWave = 3;
    [SerializeField, Min(1)] private int tier4UnlockWave = 7;
    [SerializeField, Min(0f)] private float luckTier1ReductionPerPoint = 0.25f;
    [SerializeField, Min(0f)] private float luckTier2WeightPerPoint = 0.12f;
    [SerializeField, Min(0f)] private float luckTier3WeightPerPoint = 0.08f;
    [SerializeField, Min(0f)] private float luckTier4WeightPerPoint = 0.05f;

    private ShopOfferView[] offerViews = Array.Empty<ShopOfferView>();
    private readonly List<ShopContentDefinition> currentOffers = new List<ShopContentDefinition>();
    private readonly Dictionary<string, int> purchaseCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    private Button boundRefreshButton;
    private PlayerWallet affordabilityWallet;
    private int paidRefreshCount;
    private int freeRefreshesUsed;

    public IReadOnlyList<ShopContentDefinition> CurrentOffers => currentOffers;
    public bool IsOpen { get; private set; }
    public int FreeRefreshesRemaining => Mathf.Max(0, GetPlayerStat(stats => stats.FreeRerolls) - freeRefreshesUsed);
    public int CurrentRefreshCost
    {
        get
        {
            if (FreeRefreshesRemaining > 0)
            {
                return 0;
            }

            int rawCost = Mathf.Max(0, baseRefreshCost + paidRefreshCount * refreshCostIncrease);
            return ApplyPercentagePrice(rawCost, GetPlayerStat(stats => stats.RerollPrice), false);
        }
    }

    public int GetPurchaseCount(string contentId)
    {
        return !string.IsNullOrWhiteSpace(contentId) && purchaseCounts.TryGetValue(contentId, out int count)
            ? count
            : 0;
    }

    public ShopRunSaveData CaptureRunSaveState()
    {
        var saveData = new ShopRunSaveData
        {
            paidRefreshCount = paidRefreshCount,
            freeRefreshesUsed = freeRefreshesUsed
        };

        for (int index = 0; index < currentOffers.Count; index++)
        {
            ShopContentDefinition offer = currentOffers[index];
            saveData.offerIds.Add(offer != null ? offer.Id : string.Empty);
            bool locked = index < offerViews.Length
                && offerViews[index] != null
                && offerViews[index].IsLocked;
            saveData.lockedOffers.Add(locked);
        }

        foreach (KeyValuePair<string, int> purchase in purchaseCounts)
        {
            saveData.purchaseCounts.Add(new RunPurchaseSaveEntry
            {
                contentId = purchase.Key,
                count = purchase.Value
            });
        }

        return saveData;
    }

    public void RestoreRunSaveState(ShopRunSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        EnsureUi();
        paidRefreshCount = Mathf.Max(0, saveData.paidRefreshCount);
        freeRefreshesUsed = Mathf.Max(0, saveData.freeRefreshesUsed);

        purchaseCounts.Clear();
        if (saveData.purchaseCounts != null)
        {
            foreach (RunPurchaseSaveEntry purchase in saveData.purchaseCounts)
            {
                if (purchase != null && !string.IsNullOrWhiteSpace(purchase.contentId) && purchase.count > 0)
                {
                    purchaseCounts[purchase.contentId] = purchase.count;
                }
            }
        }

        currentOffers.Clear();
        if (saveData.offerIds != null)
        {
            for (int index = 0; index < saveData.offerIds.Count; index++)
            {
                currentOffers.Add(ShopContentCatalog.FindById(saveData.offerIds[index]));
            }
        }

        for (int index = 0; index < offerViews.Length; index++)
        {
            ShopOfferView view = offerViews[index];
            if (view == null)
            {
                continue;
            }

            ShopContentDefinition offer = index < currentOffers.Count ? currentOffers[index] : null;
            if (offer == null)
            {
                view.SetVisible(false);
                continue;
            }

            bool locked = saveData.lockedOffers != null
                && index < saveData.lockedOffers.Count
                && saveData.lockedOffers[index];
            view.Bind(
                offer,
                SelectOffer,
                TryPurchaseOffer,
                GetPurchaseCount(offer.Id),
                HandleOfferLockChanged,
                locked,
                GetOfferPrice(offer),
                CanAffordOffer(offer));
            view.SetVisible(true);
        }

        UpdateRefreshButtonLabel();
        UpdateOfferPrices();
    }

    private void Awake()
    {
        EnsureRarityProfiles();
        AutoBindReferences();
    }

    private void OnEnable()
    {
        AutoBindReferences();
        BindAffordabilityWallet();
    }

    private void OnDisable()
    {
        UnbindAffordabilityWallet();
    }

    private void OnValidate()
    {
        offerCount = Mathf.Max(1, offerCount);
        baseRefreshCost = Mathf.Max(0, baseRefreshCost);
        refreshCostIncrease = Mathf.Max(0, refreshCostIncrease);
        tier2UnlockWave = Mathf.Max(1, tier2UnlockWave);
        tier3UnlockWave = Mathf.Max(tier2UnlockWave, tier3UnlockWave);
        tier4UnlockWave = Mathf.Max(tier3UnlockWave, tier4UnlockWave);
        luckTier1ReductionPerPoint = Mathf.Max(0f, luckTier1ReductionPerPoint);
        luckTier2WeightPerPoint = Mathf.Max(0f, luckTier2WeightPerPoint);
        luckTier3WeightPerPoint = Mathf.Max(0f, luckTier3WeightPerPoint);
        luckTier4WeightPerPoint = Mathf.Max(0f, luckTier4WeightPerPoint);
        EnsureRarityProfiles();
    }

    private void Start()
    {
        EnsureUi();
        SetShopVisible(startOpen);

        if (startOpen && refreshOnStart)
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
        if (shopWindowRoot == null)
        {
            Transform window = FindDescendant("ShopWindow", "Shop Window", "ShopPanel", "Shop Panel");
            shopWindowRoot = window != null ? window.gameObject : gameObject;
        }

        if (shopCanvasGroup == null && shopWindowRoot != null)
        {
            shopCanvasGroup = shopWindowRoot.GetComponent<CanvasGroup>();
            if (shopCanvasGroup == null)
            {
                shopCanvasGroup = shopWindowRoot.AddComponent<CanvasGroup>();
            }
        }

        if (shopItemContainer == null)
        {
            shopItemContainer = FindDescendant("ShopItemContainer");
        }

        if (refreshButton == null)
        {
            refreshButton = FindComponent<Button>("RefreshButton", "Refresh Button");
        }

        BindRefreshButtonTexts();

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

        if (playerWeaponEquipment == null)
        {
            playerWeaponEquipment = FindObjectOfType<PlayerWeaponEquipment>(true);
        }

        if (playerWeaponEquipment == null && Application.isPlaying)
        {
            PlayerStats playerStats = PlayerStats.Instance != null
                ? PlayerStats.Instance
                : FindObjectOfType<PlayerStats>(true);
            if (playerStats != null)
            {
                playerWeaponEquipment = playerStats.GetComponent<PlayerWeaponEquipment>();
                if (playerWeaponEquipment == null)
                {
                    playerWeaponEquipment = playerStats.gameObject.AddComponent<PlayerWeaponEquipment>();
                }
            }
        }

        if (Application.isPlaying && weaponBag != null)
        {
            weaponBag.EnsureStartingWeapon();
        }

        if (playerWeaponEquipment != null && weaponBag != null)
        {
            playerWeaponEquipment.Bind(weaponBag, weaponBag.Count > 0);
        }

        if (playerWallet == null)
        {
            playerWallet = FindObjectOfType<PlayerWallet>(true);
            if (playerWallet == null && Application.isPlaying)
            {
                playerWallet = PlayerWallet.GetOrCreate();
            }
        }

        if (currencyDisplay == null)
        {
            currencyDisplay = GetComponent<PlayerCurrencyDisplay>();
            if (currencyDisplay == null)
            {
                currencyDisplay = gameObject.AddComponent<PlayerCurrencyDisplay>();
            }
        }

        if (waveSource == null)
        {
            waveSource = FindObjectOfType<EnemySpawner>(true);
        }

        currencyDisplay.AutoBindReferences();
        if (Application.isPlaying)
        {
            currencyDisplay.BindWallet(playerWallet != null ? playerWallet : PlayerWallet.GetOrCreate());
        }
    }

    public void OpenShop()
    {
        SetShopOpen(true);
    }

    public void CloseShop()
    {
        SetShopOpen(false);
    }

    public void ToggleShop()
    {
        SetShopOpen(!IsOpen);
    }

    public void SetShopOpen(bool open)
    {
        bool opening = open && !IsOpen;
        if (open)
        {
            if (opening)
            {
                paidRefreshCount = 0;
                freeRefreshesUsed = 0;
            }

            SetShopVisible(true);
            EnsureUi();
            BindAffordabilityWallet();
            UpdateRefreshButtonLabel();
            UpdateOfferPrices();

            if (refreshWhenOpenedIfEmpty && currentOffers.Count == 0)
            {
                RefreshShop();
            }
        }
        else
        {
            SetShopVisible(false);
        }
    }

    public void RefreshShop()
    {
        EnsureUi();

        GenerateOffersPreservingLocks();
        SetStatus($"已生成 {currentOffers.Count} 个商品。点击卡片查看详情。");
    }

    public void TryPaidRefresh()
    {
        EnsureUi();

        PlayerWallet wallet = ResolvePlayerWallet();
        bool usesFreeRefresh = FreeRefreshesRemaining > 0;
        int cost = CurrentRefreshCost;
        if (wallet == null)
        {
            SetStatus("没有找到玩家金币数据，无法刷新商店。");
            return;
        }

        if (!wallet.TrySpend(cost))
        {
            SetStatus($"刷新金币不足：需要 {cost}，当前 {wallet.Coins}。");
            return;
        }

        GenerateOffersPreservingLocks();
        if (usesFreeRefresh)
        {
            freeRefreshesUsed++;
        }
        else
        {
            paidRefreshCount++;
        }

        UpdateRefreshButtonLabel();
        SetStatus(usesFreeRefresh
            ? $"已使用免费刷新；锁定商品已保留。剩余免费刷新 {FreeRefreshesRemaining} 次。"
            : $"花费 {cost} 金币刷新商店；锁定商品已保留。下次刷新需要 {CurrentRefreshCost}。");
    }

    public bool TryGenerateLootCrateReward(out ShopItemDefinition reward)
    {
        var itemPool = ShopContentCatalog.All
            .Where(content => content is ShopItemDefinition
                && !HasReachedPurchaseLimit(content))
            .ToList();
        if (itemPool.Count == 0)
        {
            reward = null;
            return false;
        }

        reward = TakeWeightedRandomOffer(itemPool, BuildLootCrateRarityWeights()) as ShopItemDefinition;
        return reward != null;
    }

    public bool TryAcceptLootCrateReward(ShopItemDefinition reward, out string failureReason)
    {
        failureReason = string.Empty;
        if (reward == null)
        {
            failureReason = "箱子奖励为空。";
            return false;
        }

        if (HasReachedPurchaseLimit(reward))
        {
            failureReason = $"{reward.LocalizedDisplayName} 已达到持有上限。";
            return false;
        }

        EnsureUi();
        if (relicBag == null)
        {
            failureReason = "未找到道具背包。";
            return false;
        }

        if (!relicBag.CanAccept(reward, out failureReason)
            || !relicBag.TryAdd(reward, out failureReason))
        {
            return false;
        }

        RegisterPurchase(reward);
        ShopItemEffectApplier.Apply(reward, PlayerStats.Instance);
        UpdateOfferPrices();
        UpdateRefreshButtonLabel();
        return true;
    }

    public int GetLootCrateRecycleValue(ShopItemDefinition reward)
    {
        if (reward == null)
        {
            return 0;
        }

        int currentPrice = GetOfferPrice(reward);
        return currentPrice > 0 ? Mathf.Max(1, Mathf.FloorToInt(currentPrice * 0.25f)) : 0;
    }

    public int RecycleLootCrateReward(ShopItemDefinition reward)
    {
        int value = GetLootCrateRecycleValue(reward);
        if (value > 0)
        {
            ResolvePlayerWallet()?.AddCoins(value);
        }

        return value;
    }

    private void GenerateOffersPreservingLocks()
    {
        var lockedOffers = new Dictionary<int, ShopContentDefinition>();
        int previousSlotCount = Mathf.Min(currentOffers.Count, offerViews.Length);
        for (int index = 0; index < previousSlotCount; index++)
        {
            ShopContentDefinition previousOffer = currentOffers[index];
            ShopOfferView previousView = offerViews[index];
            if (previousOffer != null
                && previousView != null
                && previousView.IsLocked
                && !HasReachedPurchaseLimit(previousOffer))
            {
                lockedOffers[index] = previousOffer;
            }
        }

        var lockedIds = new HashSet<string>(
            lockedOffers.Values.Select(content => content.Id),
            StringComparer.OrdinalIgnoreCase);
        var pool = ShopContentCatalog.All
            .Where(content => content != null
                && !HasReachedPurchaseLimit(content)
                && !lockedIds.Contains(content.Id))
            .ToList();
        ShopRarityWeights rarityWeights = BuildCurrentRarityWeights();

        currentOffers.Clear();
        for (int index = 0; index < offerCount; index++)
        {
            if (lockedOffers.TryGetValue(index, out ShopContentDefinition lockedOffer))
            {
                currentOffers.Add(lockedOffer);
            }
            else if (pool.Count > 0)
            {
                currentOffers.Add(TakeWeightedRandomOffer(pool, rarityWeights));
            }
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
                ShopContentDefinition offer = currentOffers[index];
                view.Bind(
                    offer,
                    SelectOffer,
                    TryPurchaseOffer,
                    GetPurchaseCount(offer.Id),
                    HandleOfferLockChanged,
                    lockedOffers.ContainsKey(index),
                    GetOfferPrice(offer),
                    CanAffordOffer(offer));
                view.SetVisible(true);
            }
            else
            {
                view.SetVisible(false);
            }
        }

    }

    private void EnsureUi()
    {
        AutoBindReferences();
        EnsureOfferViews();

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

    private void SetShopVisible(bool visible)
    {
        IsOpen = visible;

        GameObject windowRoot = shopWindowRoot != null ? shopWindowRoot : gameObject;
        if (shopCanvasGroup != null)
        {
            shopCanvasGroup.alpha = visible ? 1f : 0f;
            shopCanvasGroup.interactable = visible;
            shopCanvasGroup.blocksRaycasts = visible;
        }

        if (windowRoot != gameObject && windowRoot.activeSelf != visible)
        {
            windowRoot.SetActive(visible);
        }
    }

    private void BindRefreshButton()
    {
        if (boundRefreshButton == refreshButton)
        {
            return;
        }

        if (boundRefreshButton != null)
        {
            boundRefreshButton.onClick.RemoveListener(TryPaidRefresh);
        }

        boundRefreshButton = refreshButton;
        if (boundRefreshButton != null)
        {
            boundRefreshButton.onClick.AddListener(TryPaidRefresh);
        }

        BindRefreshButtonTexts();
        UpdateRefreshButtonLabel();
    }

    private void BindRefreshButtonTexts()
    {
        if (refreshButton == null)
        {
            return;
        }

        if (refreshCostText == null)
        {
            refreshCostText = FindChildComponent<TMP_Text>(
                refreshButton.transform,
                "CoinCost",
                "CostText",
                "Cost Text");
        }

        if (refreshLabelText == null)
        {
            refreshLabelText = FindChildComponent<TMP_Text>(
                refreshButton.transform,
                "RefreshText",
                "Refresh Text",
                "Text (TMP)");
        }
    }

    private void UpdateRefreshButtonLabel()
    {
        int cost = CurrentRefreshCost;
        PlayerWallet wallet = Application.isPlaying ? ResolvePlayerWallet() : playerWallet;
        bool canAfford = !Application.isPlaying
            || cost <= 0
            || (wallet != null && wallet.CanSpend(cost));

        if (refreshCostText != null)
        {
            string displayedCost = FreeRefreshesRemaining > 0
                ? $"0 ({FreeRefreshesRemaining})"
                : cost.ToString();
            string costColor = canAfford ? "#73E66E" : "#FF6464";
            refreshCostText.text = $"<color={costColor}>{displayedCost}</color>";
        }

        if (refreshButton == null)
        {
            return;
        }

        refreshButton.interactable = canAfford;
        if (refreshButton.targetGraphic != null)
        {
            refreshButton.targetGraphic.color = canAfford
                ? AffordableButtonColor
                : UnaffordableButtonColor;
        }

        ColorBlock colors = refreshButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = Color.white;
        refreshButton.colors = colors;
    }

    private void UpdateOfferPrices()
    {
        PlayerWallet wallet = ResolvePlayerWallet();
        int visibleCount = Mathf.Min(currentOffers.Count, offerViews.Length);
        for (int index = 0; index < visibleCount; index++)
        {
            if (offerViews[index] != null && currentOffers[index] != null)
            {
                int price = GetOfferPrice(currentOffers[index]);
                offerViews[index].SetPurchaseState(
                    price,
                    wallet != null && wallet.CanSpend(price));
            }
        }
    }

    private bool CanAffordOffer(ShopContentDefinition content)
    {
        PlayerWallet wallet = ResolvePlayerWallet();
        return wallet != null && wallet.CanSpend(GetOfferPrice(content));
    }

    private void BindAffordabilityWallet()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        PlayerWallet wallet = ResolvePlayerWallet();
        if (affordabilityWallet == wallet)
        {
            return;
        }

        UnbindAffordabilityWallet();
        affordabilityWallet = wallet;
        if (affordabilityWallet != null)
        {
            affordabilityWallet.CoinsChanged += HandleWalletCoinsChanged;
        }
    }

    private void UnbindAffordabilityWallet()
    {
        if (affordabilityWallet != null)
        {
            affordabilityWallet.CoinsChanged -= HandleWalletCoinsChanged;
            affordabilityWallet = null;
        }
    }

    private void HandleWalletCoinsChanged(PlayerWallet wallet, int coins, int delta)
    {
        UpdateOfferPrices();
        UpdateRefreshButtonLabel();
    }

    private void HandleOfferLockChanged(ShopOfferView view, bool locked)
    {
        SetStatus(locked ? "商品已锁定，刷新时会保留。" : "商品已解除锁定。");
    }

    private void SelectOffer(ShopContentDefinition content)
    {
        if (content == null)
        {
            return;
        }

        if (HasReachedPurchaseLimit(content))
        {
            SetStatus($"{content.LocalizedDisplayName} 已达到购买上限。");
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

        if (HasReachedPurchaseLimit(content))
        {
            SetStatus($"{content.LocalizedDisplayName} 已达到购买上限。");
            return;
        }

        ShopBagBase targetBag;
        string bagName;
        if (content.Kind == ShopContentKind.Weapon)
        {
            targetBag = weaponBag;
            bagName = "武器背包";
        }
        else
        {
            targetBag = relicBag;
            bagName = "道具背包";
        }

        if (targetBag == null)
        {
            SetStatus($"没有绑定 {bagName}，无法购买。");
            return;
        }

        if (!targetBag.CanAccept(content, out string failureReason))
        {
            SetStatus(failureReason);
            return;
        }

        PlayerWallet wallet = ResolvePlayerWallet();
        int price = GetOfferPrice(content);
        if (wallet == null)
        {
            SetStatus("没有找到玩家金币数据，无法购买。");
            return;
        }

        if (!wallet.TrySpend(price))
        {
            SetStatus($"金币不足：需要 {price}，当前 {wallet.Coins}。");
            return;
        }

        bool purchased;
        if (content.Kind == ShopContentKind.Weapon)
        {
            purchased = weaponBag.TryAdd(content, out failureReason);
        }
        else
        {
            purchased = relicBag.TryAdd(content, out failureReason);
        }

        if (purchased)
        {
            RegisterPurchase(content);

            ShopItemEffectResult effectResult = default;
            ShopItemDefinition item = content as ShopItemDefinition;
            bool hasItemEffectResult = item != null;
            if (hasItemEffectResult)
            {
                effectResult = ShopItemEffectApplier.Apply(item, PlayerStats.Instance);
                UpdateOfferPrices();
                UpdateRefreshButtonLabel();
            }

            if (offerView != null)
            {
                offerView.MarkPurchased();
                int purchasedIndex = Array.IndexOf(offerViews, offerView);
                if (purchasedIndex >= 0 && purchasedIndex < currentOffers.Count)
                {
                    currentOffers[purchasedIndex] = null;
                }
            }

            string effectStatus = hasItemEffectResult ? BuildItemEffectStatus(item, effectResult) : string.Empty;
            if (content.Kind == ShopContentKind.Weapon)
            {
                effectStatus += BuildWeaponCombinationStatus(weaponBag);
            }
            SetStatus($"已购买 {content.LocalizedDisplayName}，花费 {price} 金币，放入{bagName}。{effectStatus}");
        }
        else
        {
            wallet.AddCoins(price);
            SetStatus(failureReason);
        }
    }

    private bool HasReachedPurchaseLimit(ShopContentDefinition content)
    {
        ShopItemDefinition item = content as ShopItemDefinition;
        return item != null
            && item.PurchaseLimit > 0
            && GetPurchaseCount(item.Id) >= item.PurchaseLimit;
    }

    private void RegisterPurchase(ShopContentDefinition content)
    {
        if (content == null || string.IsNullOrWhiteSpace(content.Id))
        {
            return;
        }

        purchaseCounts[content.Id] = GetPurchaseCount(content.Id) + 1;
    }

    private static string BuildItemEffectStatus(ShopItemDefinition item, ShopItemEffectResult result)
    {
        if (item == null || item.Modifiers.Count == 0)
        {
            return " 该道具的特殊效果尚未接入。";
        }

        if (!result.HasPlayerStats && result.UnsupportedStats.Count > 0)
        {
            return " 未找到玩家属性组件，基础属性暂未应用。";
        }

        string applied = result.AppliedModifierCount > 0
            ? $" 已应用 {result.AppliedModifierCount} 项基础属性。"
            : "";
        string unsupported = result.UnsupportedStats.Count > 0
            ? $" 尚未支持：{string.Join("、", result.UnsupportedStats)}。"
            : "";

        return applied + unsupported;
    }

    private static string BuildWeaponCombinationStatus(WeaponBag bag)
    {
        if (bag == null || !bag.LastAddCombined || bag.LastAddedWeapon == null)
        {
            return string.Empty;
        }

        string chainLabel = bag.LastCombinationCount > 1
            ? $"，连续合成 {bag.LastCombinationCount} 次"
            : string.Empty;
        return $" 已自动合成为 {bag.LastAddedWeapon.LocalizedDisplayName}（{bag.LastAddedWeapon.RarityLabel}）{chainLabel}。";
    }

    private PlayerWallet ResolvePlayerWallet()
    {
        if (playerWallet == null)
        {
            playerWallet = PlayerWallet.GetOrCreate();
        }

        return playerWallet;
    }

    private int GetOfferPrice(ShopContentDefinition content)
    {
        return content == null
            ? 0
            : ApplyPercentagePrice(content.BasePrice, GetPlayerStat(stats => stats.ItemsPrice), true);
    }

    private static int ApplyPercentagePrice(int basePrice, int percentageModifier, bool keepPositivePrice)
    {
        if (basePrice <= 0)
        {
            return 0;
        }

        int adjustedPrice = Mathf.RoundToInt(basePrice * Mathf.Max(0f, 1f + percentageModifier / 100f));
        return keepPositivePrice ? Mathf.Max(1, adjustedPrice) : Mathf.Max(0, adjustedPrice);
    }

    private static int GetPlayerStat(Func<PlayerStats, int> selector)
    {
        PlayerStats stats = PlayerStats.Instance;
        return stats != null && selector != null ? selector(stats) : 0;
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private ShopRarityWeights BuildCurrentRarityWeights()
    {
        EnsureRarityProfiles();

        int wave = waveSource != null ? Mathf.Max(1, waveSource.CurrentWave) : 1;
        int luck = PlayerStats.Instance != null ? PlayerStats.Instance.Luck : 0;
        ShopRarityWeightProfile lower = rarityWeightProfiles[0];
        ShopRarityWeightProfile upper = rarityWeightProfiles[rarityWeightProfiles.Count - 1];

        for (int index = 0; index < rarityWeightProfiles.Count; index++)
        {
            ShopRarityWeightProfile profile = rarityWeightProfiles[index];
            if (profile.Wave <= wave)
            {
                lower = profile;
            }

            if (profile.Wave >= wave)
            {
                upper = profile;
                break;
            }
        }

        float interpolation = lower.Wave == upper.Wave
            ? 0f
            : Mathf.InverseLerp(lower.Wave, upper.Wave, wave);
        float tier1 = Mathf.Lerp(lower.GetWeight(ShopRarity.Tier1), upper.GetWeight(ShopRarity.Tier1), interpolation);
        float tier2 = Mathf.Lerp(lower.GetWeight(ShopRarity.Tier2), upper.GetWeight(ShopRarity.Tier2), interpolation);
        float tier3 = Mathf.Lerp(lower.GetWeight(ShopRarity.Tier3), upper.GetWeight(ShopRarity.Tier3), interpolation);
        float tier4 = Mathf.Lerp(lower.GetWeight(ShopRarity.Tier4), upper.GetWeight(ShopRarity.Tier4), interpolation);

        tier1 = Mathf.Max(0f, tier1 - luck * luckTier1ReductionPerPoint);
        tier2 = wave >= tier2UnlockWave
            ? Mathf.Max(0f, tier2 + luck * luckTier2WeightPerPoint)
            : 0f;
        tier3 = wave >= tier3UnlockWave
            ? Mathf.Max(0f, tier3 + luck * luckTier3WeightPerPoint)
            : 0f;
        tier4 = wave >= tier4UnlockWave
            ? Mathf.Max(0f, tier4 + luck * luckTier4WeightPerPoint)
            : 0f;

        return new ShopRarityWeights(tier1, tier2, tier3, tier4);
    }

    private ShopRarityWeights BuildLootCrateRarityWeights()
    {
        int wave = waveSource != null ? Mathf.Max(1, waveSource.CurrentWave) : 1;
        int luck = PlayerStats.Instance != null ? PlayerStats.Instance.Luck : 0;
        float luckMultiplier = Mathf.Max(0f, 1f + luck / 100f);

        float cumulativeTier2 = wave >= 2
            ? Mathf.Min(0.60f, 0.06f * (wave - 1) * luckMultiplier)
            : 0f;
        float cumulativeTier3 = wave >= 4
            ? Mathf.Min(0.25f, 0.02f * (wave - 3) * luckMultiplier)
            : 0f;
        float cumulativeTier4 = wave >= 8
            ? Mathf.Min(0.08f, 0.0023f * (wave - 7) * luckMultiplier)
            : 0f;

        float tier4 = cumulativeTier4;
        float tier3 = Mathf.Max(0f, cumulativeTier3 - tier4);
        float tier2 = Mathf.Max(0f, cumulativeTier2 - cumulativeTier3);
        float tier1 = Mathf.Max(0f, 1f - cumulativeTier2);
        return new ShopRarityWeights(tier1, tier2, tier3, tier4);
    }

    private void EnsureRarityProfiles()
    {
        if (rarityWeightProfiles == null)
        {
            rarityWeightProfiles = new List<ShopRarityWeightProfile>();
        }

        rarityWeightProfiles.RemoveAll(profile => profile == null);
        if (rarityWeightProfiles.Count == 0)
        {
            rarityWeightProfiles.Add(new ShopRarityWeightProfile(1, 90f, 10f, 0f, 0f));
            rarityWeightProfiles.Add(new ShopRarityWeightProfile(5, 60f, 28f, 10f, 0f));
            rarityWeightProfiles.Add(new ShopRarityWeightProfile(10, 40f, 35f, 20f, 5f));
        }

        foreach (ShopRarityWeightProfile profile in rarityWeightProfiles)
        {
            profile.ClampValues();
        }

        rarityWeightProfiles.Sort((left, right) => left.Wave.CompareTo(right.Wave));
    }

    private static ShopContentDefinition TakeWeightedRandomOffer(
        List<ShopContentDefinition> pool,
        ShopRarityWeights weights)
    {
        float totalWeight = 0f;
        ShopRarity lastWeightedRarity = ShopRarity.Tier1;
        for (int tier = (int)ShopRarity.Tier1; tier <= (int)ShopRarity.Tier4; tier++)
        {
            ShopRarity rarity = (ShopRarity)tier;
            float weight = weights.GetWeight(rarity);
            if (weight <= 0f || !ContainsRarity(pool, rarity))
            {
                continue;
            }

            lastWeightedRarity = rarity;
            totalWeight += weight;
        }

        if (totalWeight <= 0f)
        {
            int fallbackIndex = UnityEngine.Random.Range(0, pool.Count);
            ShopContentDefinition fallback = pool[fallbackIndex];
            pool.RemoveAt(fallbackIndex);
            return fallback;
        }

        float roll = UnityEngine.Random.value * totalWeight;
        ShopRarity selectedRarity = lastWeightedRarity;
        for (int tier = (int)ShopRarity.Tier1; tier <= (int)ShopRarity.Tier4; tier++)
        {
            ShopRarity rarity = (ShopRarity)tier;
            float weight = weights.GetWeight(rarity);
            if (weight <= 0f || !ContainsRarity(pool, rarity))
            {
                continue;
            }

            roll -= weight;
            if (roll <= 0f)
            {
                selectedRarity = rarity;
                break;
            }
        }

        int matchingCount = 0;
        for (int index = 0; index < pool.Count; index++)
        {
            if (pool[index].Rarity == selectedRarity)
            {
                matchingCount++;
            }
        }

        if (matchingCount == 0)
        {
            int fallbackIndex = UnityEngine.Random.Range(0, pool.Count);
            ShopContentDefinition fallback = pool[fallbackIndex];
            pool.RemoveAt(fallbackIndex);
            return fallback;
        }

        int selectedOrdinal = UnityEngine.Random.Range(0, matchingCount);
        for (int index = 0; index < pool.Count; index++)
        {
            if (pool[index].Rarity != selectedRarity)
            {
                continue;
            }

            if (selectedOrdinal == 0)
            {
                ShopContentDefinition selected = pool[index];
                pool.RemoveAt(index);
                return selected;
            }

            selectedOrdinal--;
        }

        int finalFallbackIndex = UnityEngine.Random.Range(0, pool.Count);
        ShopContentDefinition finalFallback = pool[finalFallbackIndex];
        pool.RemoveAt(finalFallbackIndex);
        return finalFallback;
    }

    private static bool ContainsRarity(List<ShopContentDefinition> pool, ShopRarity rarity)
    {
        for (int index = 0; index < pool.Count; index++)
        {
            if (pool[index].Rarity == rarity)
            {
                return true;
            }
        }

        return false;
    }

    private readonly struct ShopRarityWeights
    {
        private readonly float tier1;
        private readonly float tier2;
        private readonly float tier3;
        private readonly float tier4;

        public ShopRarityWeights(float tier1, float tier2, float tier3, float tier4)
        {
            this.tier1 = tier1;
            this.tier2 = tier2;
            this.tier3 = tier3;
            this.tier4 = tier4;
        }

        public float GetWeight(ShopRarity rarity)
        {
            switch (rarity)
            {
                case ShopRarity.Tier1:
                    return tier1;
                case ShopRarity.Tier2:
                    return tier2;
                case ShopRarity.Tier3:
                    return tier3;
                case ShopRarity.Tier4:
                    return tier4;
                default:
                    return 0f;
            }
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

    private static T FindChildComponent<T>(Transform root, params string[] names) where T : Component
    {
        if (root == null)
        {
            return null;
        }

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (child.name == objectName)
                {
                    return child.GetComponent<T>();
                }
            }
        }

        return null;
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
