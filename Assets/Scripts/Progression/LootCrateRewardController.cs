using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LootCrateRewardController : MonoBehaviour
{
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text iconPlaceholder;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text detailsText;
    [SerializeField] private TMP_Text pendingText;
    [SerializeField] private TMP_Text recycleButtonText;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button takeButton;
    [SerializeField] private Button recycleButton;
    private PlayerLootCrateInventory inventory;
    private ShopManager shopManager;
    private ShopItemDefinition currentReward;

    public bool IsProcessing { get; private set; }

    private void Awake()
    {
        AutoBindReferences();
        BindSceneButtons();
        if (!HasSceneReferences())
        {
            Debug.LogError("Loot crate reward UI is missing from SampleScene. Rebuild it with Tools/Potato UI/Build Reward Panels In SampleScene.", this);
        }
        SetVisible(false);
    }

    public static LootCrateRewardController FindInScene()
    {
        LootCrateRewardController existing = FindObjectOfType<LootCrateRewardController>(true);
        if (existing != null)
        {
            return existing;
        }

        Debug.LogError("LootCrateRewardController must be placed in the gameplay scene.");
        return null;
    }

    public void BeginRewards(PlayerLootCrateInventory crateInventory, ShopManager manager)
    {
        if (!HasSceneReferences())
        {
            CompleteRewards();
            return;
        }

        inventory = crateInventory;
        shopManager = manager;
        currentReward = null;

        if (inventory == null || shopManager == null || inventory.PendingCrates <= 0)
        {
            CompleteRewards();
            return;
        }

        IsProcessing = true;
        SetVisible(true);
        ShowNextReward();
    }

    private void ShowNextReward()
    {
        errorText.text = string.Empty;
        if (inventory == null || inventory.PendingCrates <= 0)
        {
            CompleteRewards();
            return;
        }

        if (!shopManager.TryGenerateLootCrateReward(out currentReward) || currentReward == null)
        {
            Debug.LogWarning("No eligible item could be generated for the pending loot crate.", this);
            CompleteRewards();
            return;
        }

        Sprite icon = currentReward.LoadIcon();
        itemIcon.sprite = icon;
        itemIcon.color = icon != null ? Color.white : Color.clear;
        iconPlaceholder.gameObject.SetActive(icon == null);
        iconPlaceholder.text = string.IsNullOrWhiteSpace(currentReward.LocalizedDisplayName)
            ? "?"
            : currentReward.LocalizedDisplayName.Substring(0, 1);

        titleText.text = currentReward.LocalizedDisplayName;
        titleText.color = GetRarityColor(currentReward.Rarity);
        rarityText.text = currentReward.RarityLabel;
        rarityText.color = GetRarityColor(currentReward.Rarity);
        string details = currentReward.BuildDetails();
        detailsText.text = string.IsNullOrWhiteSpace(details) ? "暂无详细说明" : details;
        pendingText.text = inventory.PendingCrates > 1
            ? $"战利品箱 {inventory.PendingCrates} 个 · 当前处理 1 个"
            : "最后一个战利品箱";

        int recycleValue = shopManager.GetLootCrateRecycleValue(currentReward);
        recycleButtonText.text = $"回收  +{recycleValue}";
        takeButton.interactable = true;
        recycleButton.interactable = true;
    }

    private void TakeReward()
    {
        if (!IsProcessing || currentReward == null)
        {
            return;
        }

        SetButtonsInteractable(false);
        if (!shopManager.TryAcceptLootCrateReward(currentReward, out string failureReason))
        {
            errorText.text = string.IsNullOrWhiteSpace(failureReason)
                ? "无法将该道具加入背包。"
                : failureReason;
            SetButtonsInteractable(true);
            return;
        }

        if (!inventory.TryConsumeCrate())
        {
            Debug.LogWarning("Loot crate reward was accepted but the pending crate could not be consumed.", this);
            CompleteRewards();
            return;
        }

        currentReward = null;
        ShowNextReward();
    }

    private void RecycleReward()
    {
        if (!IsProcessing || currentReward == null)
        {
            return;
        }

        SetButtonsInteractable(false);
        shopManager.RecycleLootCrateReward(currentReward);
        if (!inventory.TryConsumeCrate())
        {
            Debug.LogWarning("Loot crate reward was recycled but the pending crate could not be consumed.", this);
            CompleteRewards();
            return;
        }

        currentReward = null;
        ShowNextReward();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (takeButton != null)
        {
            takeButton.interactable = interactable;
        }

        if (recycleButton != null)
        {
            recycleButton.interactable = interactable;
        }
    }

    private void CompleteRewards()
    {
        IsProcessing = false;
        currentReward = null;
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (windowRoot != null && windowRoot.activeSelf != visible)
        {
            windowRoot.SetActive(visible);
        }
    }

    [ContextMenu("Auto Bind Scene References")]
    public void AutoBindReferences()
    {
        windowRoot = FindDescendant("LootCrateRewardWindow")?.gameObject;
        itemIcon = FindComponent<Image>("ItemIcon");
        iconPlaceholder = FindComponent<TMP_Text>("IconPlaceholder");
        titleText = FindComponent<TMP_Text>("ItemName");
        rarityText = FindComponent<TMP_Text>("Rarity");
        detailsText = FindComponent<TMP_Text>("Details");
        pendingText = FindComponent<TMP_Text>("Pending");
        errorText = FindComponent<TMP_Text>("Error");
        takeButton = FindComponent<Button>("TakeButton");
        recycleButton = FindComponent<Button>("RecycleButton");
        recycleButtonText = recycleButton != null ? recycleButton.GetComponentInChildren<TMP_Text>(true) : null;
    }

    private void BindSceneButtons()
    {
        if (takeButton != null)
        {
            takeButton.onClick.RemoveListener(TakeReward);
            takeButton.onClick.AddListener(TakeReward);
        }
        if (recycleButton != null)
        {
            recycleButton.onClick.RemoveListener(RecycleReward);
            recycleButton.onClick.AddListener(RecycleReward);
        }
    }

    private bool HasSceneReferences()
    {
        return windowRoot != null && itemIcon != null && iconPlaceholder != null && titleText != null
            && rarityText != null && detailsText != null && pendingText != null && recycleButtonText != null
            && errorText != null && takeButton != null && recycleButton != null;
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

    private static Color GetRarityColor(ShopRarity rarity)
    {
        switch (rarity)
        {
            case ShopRarity.Tier2:
                return new Color(0.35f, 0.95f, 0.5f, 1f);
            case ShopRarity.Tier3:
                return new Color(0.4f, 0.65f, 1f, 1f);
            case ShopRarity.Tier4:
                return new Color(0.85f, 0.45f, 1f, 1f);
            default:
                return Color.white;
        }
    }

}
