using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelUpRewardController : MonoBehaviour
{
    private const string FontResourcePath = "Fonts & Materials/SmileySans-Oblique SDF";

    private sealed class OptionView
    {
        public Button Button;
        public Image Background;
        public TMP_Text TierText;
        public TMP_Text NameText;
        public TMP_Text ValueText;
        public TMP_Text CurrentText;
    }

    private readonly List<OptionView> optionViews = new List<OptionView>();
    private IReadOnlyList<LevelUpUpgradeOption> currentOptions = Array.Empty<LevelUpUpgradeOption>();
    private GameObject windowRoot;
    private TMP_Text titleText;
    private TMP_Text pendingText;
    private Button rerollButton;
    private TMP_Text rerollText;
    private PlayerStatsPanelView statsPanelView;
    private PlayerExperience experience;
    private PlayerStats playerStats;
    private PlayerWallet wallet;
    private int currentRewardLevel;
    private int wave;
    private int paidRerollCount;

    public bool IsProcessing { get; private set; }

    private void Awake()
    {
        BuildUi();
        SetVisible(false);
    }

    private void OnDestroy()
    {
        BindWallet(null);
    }

    public static LevelUpRewardController GetOrCreate()
    {
        LevelUpRewardController existing = FindObjectOfType<LevelUpRewardController>(true);
        if (existing != null)
        {
            return existing;
        }

        GameObject controllerObject = new GameObject("LevelUpRewardController");
        return controllerObject.AddComponent<LevelUpRewardController>();
    }

    public void BeginRewards(PlayerExperience playerExperience, int currentWave)
    {
        experience = playerExperience;
        playerStats = PlayerStats.Instance;
        wave = Mathf.Max(1, currentWave);
        paidRerollCount = 0;

        if (experience == null || playerStats == null || experience.PendingUpgradeCount <= 0)
        {
            CompleteRewards();
            return;
        }

        BindWallet(PlayerWallet.GetOrCreate());
        IsProcessing = true;
        SetVisible(true);
        statsPanelView?.BindPlayerStats(playerStats);
        ShowNextReward();
    }

    private void ShowNextReward()
    {
        if (experience == null || !experience.TryPeekPendingUpgrade(out currentRewardLevel))
        {
            CompleteRewards();
            return;
        }

        int luck = playerStats != null ? playerStats.Luck : 0;
        currentOptions = LevelUpUpgradeCatalog.GenerateOptions(currentRewardLevel, luck, optionViews.Count);
        titleText.text = $"升级奖励 · 等级 {currentRewardLevel}";
        pendingText.text = experience.PendingUpgradeCount > 1
            ? $"选择一项属性强化（剩余 {experience.PendingUpgradeCount} 次）"
            : "选择一项属性强化";

        for (int index = 0; index < optionViews.Count; index++)
        {
            OptionView view = optionViews[index];
            bool hasOption = index < currentOptions.Count;
            view.Button.gameObject.SetActive(hasOption);
            if (!hasOption)
            {
                continue;
            }

            LevelUpUpgradeOption option = currentOptions[index];
            Color tierColor = LevelUpUpgradeCatalog.GetTierColor(option.Tier);
            view.Background.color = tierColor;
            view.TierText.text = LevelUpUpgradeCatalog.GetTierLabel(option.Tier);
            view.NameText.text = option.DisplayName;
            view.ValueText.text = $"+{option.Value}{GetValueSuffix(option.StatId)}";
            int currentValue = playerStats.GetStat(option.StatId);
            view.CurrentText.text = $"当前 {currentValue}  →  {currentValue + option.Value}";
        }

        UpdateRerollButton();
    }

    private void SelectOption(int optionIndex)
    {
        if (!IsProcessing || optionIndex < 0 || optionIndex >= currentOptions.Count)
        {
            return;
        }

        LevelUpUpgradeOption option = currentOptions[optionIndex];
        playerStats.AddStat(option.StatId, option.Value);
        if (!experience.TryConsumePendingUpgrade(currentRewardLevel))
        {
            Debug.LogWarning("Level-up reward queue changed before the selected reward was consumed.", this);
            CompleteRewards();
            return;
        }

        ShowNextReward();
    }

    private void Reroll()
    {
        if (!IsProcessing || wallet == null)
        {
            return;
        }

        int cost = GetRerollCost();
        if (!wallet.TrySpend(cost))
        {
            UpdateRerollButton();
            return;
        }

        paidRerollCount++;
        ShowNextReward();
    }

    private int GetRerollCost()
    {
        int baseCost = wave + wave / 2;
        int increase = Mathf.Max(1, wave / 2);
        int rawCost = baseCost + paidRerollCount * increase;
        int modifier = playerStats != null ? playerStats.RerollPrice : 0;
        return Mathf.Max(1, Mathf.FloorToInt(rawCost * Mathf.Max(0f, 1f + modifier / 100f)));
    }

    private void UpdateRerollButton()
    {
        if (rerollButton == null || rerollText == null)
        {
            return;
        }

        int cost = GetRerollCost();
        bool affordable = wallet != null && wallet.CanSpend(cost);
        rerollButton.interactable = affordable;
        rerollText.text = $"重随  {cost}";
        rerollText.color = affordable
            ? new Color(0.38f, 1f, 0.4f, 1f)
            : new Color(1f, 0.32f, 0.32f, 1f);
    }

    private void CompleteRewards()
    {
        IsProcessing = false;
        SetVisible(false);
        BindWallet(null);
    }

    private void HandleCoinsChanged(PlayerWallet changedWallet, int coins, int delta)
    {
        UpdateRerollButton();
    }

    private void BindWallet(PlayerWallet newWallet)
    {
        if (wallet == newWallet)
        {
            return;
        }

        if (wallet != null)
        {
            wallet.CoinsChanged -= HandleCoinsChanged;
        }

        wallet = newWallet;
        if (wallet != null)
        {
            wallet.CoinsChanged += HandleCoinsChanged;
        }
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
            "LevelUpCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        canvasObject.layer = 5;
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        windowRoot = CreateUiObject("LevelUpWindow", canvasObject.transform);
        Stretch(windowRoot.GetComponent<RectTransform>());
        Image dimmer = windowRoot.AddComponent<Image>();
        dimmer.color = new Color(0.015f, 0.018f, 0.025f, 0.9f);

        GameObject panel = CreateUiObject("Panel", windowRoot.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        Center(panelRect, new Vector2(1760f, 820f));
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.07f, 0.075f, 0.09f, 0.98f);

        titleText = CreateText("Title", panel.transform, font, 44f, FontStyles.Bold);
        SetRect(titleText.rectTransform, new Vector2(0.04f, 0.87f), new Vector2(0.72f, 0.98f));
        titleText.alignment = TextAlignmentOptions.Center;

        pendingText = CreateText("Pending", panel.transform, font, 23f, FontStyles.Normal);
        SetRect(pendingText.rectTransform, new Vector2(0.04f, 0.79f), new Vector2(0.72f, 0.87f));
        pendingText.alignment = TextAlignmentOptions.Center;
        pendingText.color = new Color(0.78f, 0.8f, 0.86f, 1f);

        GameObject optionsRoot = CreateUiObject("Options", panel.transform);
        SetRect(optionsRoot.GetComponent<RectTransform>(), new Vector2(0.025f, 0.18f), new Vector2(0.74f, 0.77f));
        HorizontalLayoutGroup layout = optionsRoot.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 16f;
        layout.padding = new RectOffset(12, 12, 8, 8);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        for (int index = 0; index < 4; index++)
        {
            int capturedIndex = index;
            optionViews.Add(CreateOptionView(optionsRoot.transform, font, () => SelectOption(capturedIndex)));
        }

        GameObject rerollObject = CreateUiObject("RerollButton", panel.transform);
        RectTransform rerollRect = rerollObject.GetComponent<RectTransform>();
        rerollRect.anchorMin = new Vector2(0.3825f, 0.04f);
        rerollRect.anchorMax = new Vector2(0.3825f, 0.14f);
        rerollRect.offsetMin = new Vector2(-145f, 0f);
        rerollRect.offsetMax = new Vector2(145f, 0f);
        Image rerollImage = rerollObject.AddComponent<Image>();
        rerollImage.color = new Color(0.15f, 0.16f, 0.2f, 1f);
        rerollButton = rerollObject.AddComponent<Button>();
        rerollButton.targetGraphic = rerollImage;
        rerollButton.onClick.AddListener(Reroll);
        rerollText = CreateText("Text", rerollObject.transform, font, 25f, FontStyles.Bold);
        Stretch(rerollText.rectTransform);
        rerollText.alignment = TextAlignmentOptions.Center;

        GameObject divider = CreateUiObject("StatsDivider", panel.transform);
        SetRect(divider.GetComponent<RectTransform>(), new Vector2(0.752f, 0.04f), new Vector2(0.754f, 0.96f));
        divider.AddComponent<Image>().color = new Color(0.24f, 0.26f, 0.31f, 1f);
        statsPanelView = CreateStatsPanel(panel.transform, font);
    }

    private static PlayerStatsPanelView CreateStatsPanel(Transform parent, TMP_FontAsset font)
    {
        GameObject statsPanel = CreateUiObject("UpgradeStatsPanel", parent);
        SetRect(statsPanel.GetComponent<RectTransform>(), new Vector2(0.765f, 0.035f), new Vector2(0.985f, 0.965f));
        Image background = statsPanel.AddComponent<Image>();
        background.color = new Color(0.045f, 0.05f, 0.065f, 0.98f);

        TMP_Text title = CreateText("TitleText", statsPanel.transform, font, 30f, FontStyles.Bold);
        SetRect(title.rectTransform, new Vector2(0.05f, 0.925f), new Vector2(0.95f, 0.99f));
        title.text = "属性";
        title.alignment = TextAlignmentOptions.Center;

        TMP_Text primaryTab = CreateText("PrimaryTabText", statsPanel.transform, font, 21f, FontStyles.Bold);
        SetRect(primaryTab.rectTransform, new Vector2(0.08f, 0.855f), new Vector2(0.48f, 0.92f));
        primaryTab.text = "主要";
        primaryTab.alignment = TextAlignmentOptions.Center;
        primaryTab.raycastTarget = true;
        Button primaryButton = primaryTab.gameObject.AddComponent<Button>();
        primaryButton.targetGraphic = primaryTab;
        primaryButton.transition = Selectable.Transition.None;

        TMP_Text secondaryTab = CreateText("SecondaryTabText", statsPanel.transform, font, 21f, FontStyles.Bold);
        SetRect(secondaryTab.rectTransform, new Vector2(0.52f, 0.855f), new Vector2(0.92f, 0.92f));
        secondaryTab.text = "次要";
        secondaryTab.alignment = TextAlignmentOptions.Center;
        secondaryTab.raycastTarget = true;
        Button secondaryButton = secondaryTab.gameObject.AddComponent<Button>();
        secondaryButton.targetGraphic = secondaryTab;
        secondaryButton.transition = Selectable.Transition.None;

        GameObject viewportObject = CreateUiObject("RowsViewport", statsPanel.transform);
        SetRect(viewportObject.GetComponent<RectTransform>(), new Vector2(0.035f, 0.035f), new Vector2(0.965f, 0.845f));
        Image viewportImage = viewportObject.AddComponent<Image>();
        viewportImage.color = new Color(0f, 0f, 0f, 0.01f);
        viewportObject.AddComponent<RectMask2D>();
        ScrollRect scrollRect = viewportObject.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 24f;

        GameObject contentObject = CreateUiObject("RowsContent", viewportObject.transform);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;
        VerticalLayoutGroup rowsLayout = contentObject.AddComponent<VerticalLayoutGroup>();
        rowsLayout.padding = new RectOffset(4, 4, 4, 4);
        rowsLayout.spacing = 3f;
        rowsLayout.childAlignment = TextAnchor.UpperCenter;
        rowsLayout.childControlWidth = true;
        rowsLayout.childControlHeight = true;
        rowsLayout.childForceExpandWidth = true;
        rowsLayout.childForceExpandHeight = false;
        ContentSizeFitter contentFitter = contentObject.AddComponent<ContentSizeFitter>();
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.viewport = viewportObject.GetComponent<RectTransform>();
        scrollRect.content = contentRect;

        GameObject rowTemplateObject = CreateUiObject("RowTemplate", contentObject.transform);
        Image rowBackground = rowTemplateObject.AddComponent<Image>();
        rowBackground.color = new Color(0.09f, 0.095f, 0.115f, 0.92f);
        LayoutElement rowLayout = rowTemplateObject.AddComponent<LayoutElement>();
        rowLayout.minHeight = 38f;
        rowLayout.preferredHeight = 38f;

        TMP_Text icon = CreateText("IconText", rowTemplateObject.transform, font, 18f, FontStyles.Bold);
        SetRect(icon.rectTransform, new Vector2(0.02f, 0f), new Vector2(0.15f, 1f));
        icon.alignment = TextAlignmentOptions.Center;

        TMP_Text statName = CreateText("NameText", rowTemplateObject.transform, font, 17f, FontStyles.Normal);
        SetRect(statName.rectTransform, new Vector2(0.16f, 0f), new Vector2(0.73f, 1f));
        statName.alignment = TextAlignmentOptions.MidlineLeft;
        statName.enableAutoSizing = true;
        statName.fontSizeMin = 12f;
        statName.fontSizeMax = 17f;

        TMP_Text value = CreateText("ValueText", rowTemplateObject.transform, font, 18f, FontStyles.Bold);
        SetRect(value.rectTransform, new Vector2(0.74f, 0f), new Vector2(0.97f, 1f));
        value.alignment = TextAlignmentOptions.MidlineRight;

        rowTemplateObject.AddComponent<PlayerStatRowView>();
        rowTemplateObject.SetActive(false);

        return statsPanel.AddComponent<PlayerStatsPanelView>();
    }

    private static OptionView CreateOptionView(
        Transform parent,
        TMP_FontAsset font,
        UnityEngine.Events.UnityAction clickAction)
    {
        GameObject card = CreateUiObject("UpgradeOption", parent);
        Image background = card.AddComponent<Image>();
        Button button = card.AddComponent<Button>();
        button.targetGraphic = background;
        button.onClick.AddListener(clickAction);

        TMP_Text tier = CreateText("Tier", card.transform, font, 22f, FontStyles.Bold);
        SetRect(tier.rectTransform, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.95f));
        tier.alignment = TextAlignmentOptions.Center;

        TMP_Text name = CreateText("Name", card.transform, font, 31f, FontStyles.Bold);
        SetRect(name.rectTransform, new Vector2(0.08f, 0.5f), new Vector2(0.92f, 0.78f));
        name.alignment = TextAlignmentOptions.Center;

        TMP_Text value = CreateText("Value", card.transform, font, 46f, FontStyles.Bold);
        SetRect(value.rectTransform, new Vector2(0.08f, 0.25f), new Vector2(0.92f, 0.52f));
        value.alignment = TextAlignmentOptions.Center;
        value.color = new Color(0.28f, 1f, 0.35f, 1f);

        TMP_Text current = CreateText("Current", card.transform, font, 19f, FontStyles.Normal);
        SetRect(current.rectTransform, new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.23f));
        current.alignment = TextAlignmentOptions.Center;
        current.color = new Color(0.92f, 0.92f, 0.94f, 1f);

        return new OptionView
        {
            Button = button,
            Background = background,
            TierText = tier,
            NameText = name,
            ValueText = value,
            CurrentText = current,
        };
    }

    private static string GetValueSuffix(PlayerStatId statId)
    {
        switch (statId)
        {
            case PlayerStatId.LifeSteal:
            case PlayerStatId.Damage:
            case PlayerStatId.AttackSpeed:
            case PlayerStatId.CritChance:
            case PlayerStatId.Dodge:
            case PlayerStatId.Speed:
                return "%";
            default:
                return string.Empty;
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
