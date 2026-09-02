using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LootCrateRewardController : MonoBehaviour
{
    private const string FontResourcePath = "Fonts & Materials/SmileySans-Oblique SDF";

    private GameObject windowRoot;
    private Image itemIcon;
    private TMP_Text iconPlaceholder;
    private TMP_Text titleText;
    private TMP_Text rarityText;
    private TMP_Text detailsText;
    private TMP_Text pendingText;
    private TMP_Text recycleButtonText;
    private TMP_Text errorText;
    private Button takeButton;
    private Button recycleButton;
    private PlayerLootCrateInventory inventory;
    private ShopManager shopManager;
    private ShopItemDefinition currentReward;

    public bool IsProcessing { get; private set; }

    private void Awake()
    {
        BuildUi();
        SetVisible(false);
    }

    public static LootCrateRewardController GetOrCreate()
    {
        LootCrateRewardController existing = FindObjectOfType<LootCrateRewardController>(true);
        if (existing != null)
        {
            return existing;
        }

        GameObject controllerObject = new GameObject("LootCrateRewardController");
        return controllerObject.AddComponent<LootCrateRewardController>();
    }

    public void BeginRewards(PlayerLootCrateInventory crateInventory, ShopManager manager)
    {
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

    private void BuildUi()
    {
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>(FontResourcePath);
        GameObject canvasObject = new GameObject(
            "LootCrateRewardCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        canvasObject.layer = 5;
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 190;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        windowRoot = CreateUiObject("LootCrateRewardWindow", canvasObject.transform);
        Stretch(windowRoot.GetComponent<RectTransform>());
        Image dimmer = windowRoot.AddComponent<Image>();
        dimmer.color = new Color(0.015f, 0.018f, 0.025f, 0.9f);

        GameObject panel = CreateUiObject("Panel", windowRoot.transform);
        Center(panel.GetComponent<RectTransform>(), new Vector2(940f, 760f));
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.07f, 0.075f, 0.09f, 0.99f);

        TMP_Text header = CreateText("Header", panel.transform, font, 42f, FontStyles.Bold);
        SetRect(header.rectTransform, new Vector2(0.05f, 0.89f), new Vector2(0.95f, 0.98f));
        header.text = "战利品箱";
        header.alignment = TextAlignmentOptions.Center;

        pendingText = CreateText("Pending", panel.transform, font, 21f, FontStyles.Normal);
        SetRect(pendingText.rectTransform, new Vector2(0.05f, 0.83f), new Vector2(0.95f, 0.89f));
        pendingText.alignment = TextAlignmentOptions.Center;
        pendingText.color = new Color(0.78f, 0.8f, 0.86f, 1f);

        GameObject iconObject = CreateUiObject("ItemIcon", panel.transform);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.08f, 0.52f);
        iconRect.anchorMax = new Vector2(0.34f, 0.82f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        itemIcon = iconObject.AddComponent<Image>();
        itemIcon.preserveAspect = true;
        itemIcon.raycastTarget = false;

        iconPlaceholder = CreateText("IconPlaceholder", iconObject.transform, font, 96f, FontStyles.Bold);
        Stretch(iconPlaceholder.rectTransform);
        iconPlaceholder.alignment = TextAlignmentOptions.Center;
        iconPlaceholder.color = new Color(0.85f, 0.86f, 0.9f, 1f);

        titleText = CreateText("ItemName", panel.transform, font, 36f, FontStyles.Bold);
        SetRect(titleText.rectTransform, new Vector2(0.39f, 0.7f), new Vector2(0.92f, 0.82f));
        titleText.alignment = TextAlignmentOptions.Left;

        rarityText = CreateText("Rarity", panel.transform, font, 23f, FontStyles.Bold);
        SetRect(rarityText.rectTransform, new Vector2(0.39f, 0.62f), new Vector2(0.92f, 0.7f));
        rarityText.alignment = TextAlignmentOptions.Left;

        detailsText = CreateText("Details", panel.transform, font, 23f, FontStyles.Normal);
        SetRect(detailsText.rectTransform, new Vector2(0.08f, 0.27f), new Vector2(0.92f, 0.59f));
        detailsText.alignment = TextAlignmentOptions.TopLeft;
        detailsText.enableAutoSizing = true;
        detailsText.fontSizeMin = 15f;
        detailsText.fontSizeMax = 23f;
        detailsText.overflowMode = TextOverflowModes.Ellipsis;

        takeButton = CreateButton(
            "TakeButton",
            panel.transform,
            font,
            "收下",
            new Vector2(0.12f, 0.09f),
            new Vector2(0.46f, 0.21f),
            new Color(0.18f, 0.58f, 0.24f, 1f),
            TakeReward,
            out _);

        recycleButton = CreateButton(
            "RecycleButton",
            panel.transform,
            font,
            "回收",
            new Vector2(0.54f, 0.09f),
            new Vector2(0.88f, 0.21f),
            new Color(0.58f, 0.36f, 0.14f, 1f),
            RecycleReward,
            out recycleButtonText);

        errorText = CreateText("Error", panel.transform, font, 18f, FontStyles.Bold);
        SetRect(errorText.rectTransform, new Vector2(0.08f, 0.015f), new Vector2(0.92f, 0.075f));
        errorText.alignment = TextAlignmentOptions.Center;
        errorText.color = new Color(1f, 0.32f, 0.32f, 1f);
    }

    private static Button CreateButton(
        string objectName,
        Transform parent,
        TMP_FontAsset font,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Color color,
        UnityEngine.Events.UnityAction action,
        out TMP_Text labelText)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        SetRect(buttonObject.GetComponent<RectTransform>(), anchorMin, anchorMax);
        Image image = buttonObject.AddComponent<Image>();
        image.color = color;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        labelText = CreateText("Text", buttonObject.transform, font, 27f, FontStyles.Bold);
        Stretch(labelText.rectTransform);
        labelText.text = label;
        labelText.alignment = TextAlignmentOptions.Center;
        return button;
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

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject result = new GameObject(objectName, typeof(RectTransform));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static TMP_Text CreateText(
        string objectName,
        Transform parent,
        TMP_FontAsset font,
        float fontSize,
        FontStyles style)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        if (font != null)
        {
            text.font = font;
        }

        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Stretch(RectTransform rect)
    {
        SetRect(rect, Vector2.zero, Vector2.one);
    }

    private static void Center(RectTransform rect, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
    }
}
