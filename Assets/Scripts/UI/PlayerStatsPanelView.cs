using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PlayerStatDisplayEntry
{
    public string Id;
    public string DisplayName;
    public string IconText;
    public Sprite IconSprite;
    public string ValueText;
    public Color IconColor;
    public Color NameColor;
    public Color ValueColor;

    public PlayerStatDisplayEntry(
        string id,
        string displayName,
        string iconText,
        string valueText,
        Color iconColor,
        bool highlight = true)
    {
        Id = id;
        DisplayName = displayName;
        IconText = iconText;
        IconSprite = null;
        ValueText = valueText;
        IconColor = iconColor;
        NameColor = highlight ? PlayerStatsPanelView.PositiveGreen : Color.white;
        ValueColor = highlight ? PlayerStatsPanelView.PositiveGreen : Color.white;
    }
}

public sealed class PlayerStatsPanelView : MonoBehaviour
{
    private enum StatsPage
    {
        Primary,
        Secondary
    }

    public static readonly Color PositiveGreen = new Color(0.05f, 1f, 0.12f, 1f);
    public static readonly Color NegativeRed = new Color(1f, 0.18f, 0.18f, 1f);
    public static readonly Color SoftWhite = new Color(0.92f, 0.92f, 0.92f, 1f);

    [Header("Panel References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text primaryTabText;
    [SerializeField] private TMP_Text secondaryTabText;
    [SerializeField] private Button primaryTabButton;
    [SerializeField] private Button secondaryTabButton;
    [SerializeField] private Transform rowsContainer;
    [SerializeField] private PlayerStatRowView rowTemplate;
    [SerializeField] private ScrollRect rowsScrollRect;

    [Header("Preview Data")]
    [SerializeField] private bool renderPreviewOnStart = true;
    [SerializeField] private bool bindPlayerStatsOnStart = true;
    [SerializeField] private List<PlayerStatDisplayEntry> previewRows = new List<PlayerStatDisplayEntry>();
    [SerializeField] private List<PlayerStatDisplayEntry> secondaryPreviewRows = new List<PlayerStatDisplayEntry>();

    private readonly List<PlayerStatRowView> spawnedRows = new List<PlayerStatRowView>();
    private readonly List<PlayerStatDisplayEntry> primaryRows = new List<PlayerStatDisplayEntry>();
    private readonly List<PlayerStatDisplayEntry> secondaryRows = new List<PlayerStatDisplayEntry>();
    private PlayerStats boundPlayerStats;
    private StatsPage currentPage = StatsPage.Primary;

    private void Awake()
    {
        AutoBindReferences();
        EnsureScrollableRows();
        EnsureTabButtons();
        BindTabButtons();
        CollectExistingRows();
        EnsureDefaultPreviewRows();
        UpdateTabVisuals();
    }

    private void Start()
    {
        if (bindPlayerStatsOnStart)
        {
            BindPlayerStats(PlayerStats.Instance);
        }

        if (boundPlayerStats == null && renderPreviewOnStart)
        {
            SetPageRows(previewRows, secondaryPreviewRows);
        }
    }

    private void OnEnable()
    {
        if (bindPlayerStatsOnStart && boundPlayerStats == null && PlayerStats.Instance != null)
        {
            BindPlayerStats(PlayerStats.Instance);
        }
    }

    private void OnDisable()
    {
        UnbindPlayerStats();
    }

    private void OnDestroy()
    {
        UnbindTabButtons();
    }

    private void Reset()
    {
        AutoBindReferences();
        EnsureDefaultPreviewRows();
    }

    private void OnValidate()
    {
        AutoBindReferences();
        EnsureDefaultPreviewRows();
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        if (titleText == null)
        {
            titleText = FindComponent<TMP_Text>("TitleText", "Title");
        }

        if (primaryTabText == null)
        {
            primaryTabText = FindComponent<TMP_Text>("PrimaryTabText", "PrimaryTab");
        }

        if (secondaryTabText == null)
        {
            secondaryTabText = FindComponent<TMP_Text>("SecondaryTabText", "SecondaryTab");
        }

        if (primaryTabButton == null && primaryTabText != null)
        {
            primaryTabButton = primaryTabText.GetComponent<Button>();
        }

        if (secondaryTabButton == null && secondaryTabText != null)
        {
            secondaryTabButton = secondaryTabText.GetComponent<Button>();
            if (secondaryTabButton == null && secondaryTabText.transform.parent != null)
            {
                secondaryTabButton = secondaryTabText.transform.parent.GetComponent<Button>();
            }
        }

        if (rowsContainer == null)
        {
            rowsContainer = FindDescendant("RowsContent", "RowsContainer", "Rows");
        }

        if (rowsScrollRect == null && rowsContainer != null)
        {
            rowsScrollRect = rowsContainer.GetComponentInParent<ScrollRect>(true);
        }

        if (rowTemplate == null)
        {
            rowTemplate = FindComponent<PlayerStatRowView>("RowTemplate");
        }
    }

    public void SetRows(IReadOnlyList<PlayerStatDisplayEntry> rows)
    {
        AutoBindReferences();
        ReplaceRows(primaryRows, rows);
        RenderCurrentPage();
    }

    public void SetSecondaryRows(IReadOnlyList<PlayerStatDisplayEntry> rows)
    {
        AutoBindReferences();
        ReplaceRows(secondaryRows, rows);
        RenderCurrentPage();
    }

    public void ShowPrimaryStats()
    {
        SetCurrentPage(StatsPage.Primary);
    }

    public void ShowSecondaryStats()
    {
        SetCurrentPage(StatsPage.Secondary);
    }

    private void RenderCurrentPage()
    {
        IReadOnlyList<PlayerStatDisplayEntry> visibleRows = currentPage == StatsPage.Primary
            ? primaryRows
            : secondaryRows;

        if (titleText != null)
        {
            titleText.text = "属性";
        }

        if (primaryTabText != null)
        {
            primaryTabText.text = "主要";
        }

        if (secondaryTabText != null)
        {
            secondaryTabText.text = "次要";
        }

        if (rowsContainer == null || rowTemplate == null)
        {
            return;
        }

        CollectExistingRows();
        rowTemplate.gameObject.SetActive(false);
        EnsureRowCount(visibleRows.Count);

        for (int index = 0; index < spawnedRows.Count; index++)
        {
            bool visible = index < visibleRows.Count;
            spawnedRows[index].gameObject.SetActive(visible);
            if (visible)
            {
                spawnedRows[index].Bind(visibleRows[index]);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rowsContainer);
        if (rowsScrollRect != null)
        {
            rowsScrollRect.verticalNormalizedPosition = 1f;
        }

        UpdateTabVisuals();
    }

    public void BindPlayerStats(PlayerStats playerStats)
    {
        if (boundPlayerStats == playerStats)
        {
            RefreshFromPlayerStats();
            return;
        }

        UnbindPlayerStats();
        boundPlayerStats = playerStats;
        if (boundPlayerStats != null)
        {
            boundPlayerStats.StatsChanged += HandlePlayerStatsChanged;
            RefreshFromPlayerStats();
        }
    }

    public void SetValue(string id, string valueText)
    {
        EnsureDefaultPreviewRows();
        TrySetPreviewValue(previewRows, id, valueText);
        TrySetPreviewValue(secondaryPreviewRows, id, valueText);
        SetPageRows(previewRows, secondaryPreviewRows);
    }

    public void RefreshFromPlayerStats()
    {
        if (boundPlayerStats != null)
        {
            SetPageRows(
                boundPlayerStats.BuildDisplayEntries(),
                boundPlayerStats.BuildSecondaryDisplayEntries());
        }
    }

    private void HandlePlayerStatsChanged(PlayerStats playerStats)
    {
        if (playerStats == boundPlayerStats)
        {
            RefreshFromPlayerStats();
        }
    }

    private void UnbindPlayerStats()
    {
        if (boundPlayerStats != null)
        {
            boundPlayerStats.StatsChanged -= HandlePlayerStatsChanged;
            boundPlayerStats = null;
        }
    }

    private void SetCurrentPage(StatsPage page)
    {
        if (currentPage == page)
        {
            UpdateTabVisuals();
            return;
        }

        currentPage = page;
        RenderCurrentPage();
    }

    private void SetPageRows(
        IReadOnlyList<PlayerStatDisplayEntry> newPrimaryRows,
        IReadOnlyList<PlayerStatDisplayEntry> newSecondaryRows)
    {
        ReplaceRows(primaryRows, newPrimaryRows);
        ReplaceRows(secondaryRows, newSecondaryRows);
        RenderCurrentPage();
    }

    private static void ReplaceRows(
        ICollection<PlayerStatDisplayEntry> destination,
        IReadOnlyList<PlayerStatDisplayEntry> source)
    {
        destination.Clear();
        if (source == null)
        {
            return;
        }

        for (int index = 0; index < source.Count; index++)
        {
            destination.Add(source[index]);
        }
    }

    private void EnsureScrollableRows()
    {
        if (rowsContainer == null)
        {
            return;
        }

        if (rowsContainer.name == "RowsContent")
        {
            rowsScrollRect = rowsContainer.GetComponentInParent<ScrollRect>(true);
            return;
        }

        if (!Application.isPlaying)
        {
            return;
        }

        RectTransform viewport = rowsContainer as RectTransform;
        if (viewport == null)
        {
            return;
        }

        var existingChildren = new List<Transform>();
        for (int index = 0; index < viewport.childCount; index++)
        {
            existingChildren.Add(viewport.GetChild(index));
        }

        GameObject contentObject = new GameObject("RowsContent", typeof(RectTransform));
        contentObject.layer = viewport.gameObject.layer;
        RectTransform content = contentObject.GetComponent<RectTransform>();
        content.SetParent(viewport, false);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;

        foreach (Transform child in existingChildren)
        {
            child.SetParent(content, false);
        }

        VerticalLayoutGroup oldLayout = viewport.GetComponent<VerticalLayoutGroup>();
        VerticalLayoutGroup newLayout = contentObject.AddComponent<VerticalLayoutGroup>();
        if (oldLayout != null)
        {
            newLayout.padding = oldLayout.padding;
            newLayout.spacing = oldLayout.spacing;
            newLayout.childAlignment = oldLayout.childAlignment;
            newLayout.childControlWidth = oldLayout.childControlWidth;
            newLayout.childControlHeight = oldLayout.childControlHeight;
            newLayout.childForceExpandWidth = oldLayout.childForceExpandWidth;
            newLayout.childForceExpandHeight = oldLayout.childForceExpandHeight;
            oldLayout.enabled = false;
            Destroy(oldLayout);
        }

        ContentSizeFitter fitter = contentObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        if (viewport.GetComponent<RectMask2D>() == null)
        {
            viewport.gameObject.AddComponent<RectMask2D>();
        }

        Image viewportGraphic = viewport.GetComponent<Image>();
        if (viewportGraphic == null)
        {
            viewportGraphic = viewport.gameObject.AddComponent<Image>();
        }

        viewportGraphic.color = Color.clear;
        viewportGraphic.raycastTarget = true;

        rowsScrollRect = viewport.GetComponent<ScrollRect>();
        if (rowsScrollRect == null)
        {
            rowsScrollRect = viewport.gameObject.AddComponent<ScrollRect>();
        }

        rowsScrollRect.viewport = viewport;
        rowsScrollRect.content = content;
        rowsScrollRect.horizontal = false;
        rowsScrollRect.vertical = true;
        rowsScrollRect.movementType = ScrollRect.MovementType.Clamped;
        rowsScrollRect.scrollSensitivity = 24f;
        rowsContainer = content;
    }

    private void EnsureTabButtons()
    {
        primaryTabButton = EnsureTabButton(primaryTabButton, primaryTabText, false);
        secondaryTabButton = EnsureTabButton(secondaryTabButton, secondaryTabText, true);
    }

    private static Button EnsureTabButton(Button button, TMP_Text label, bool preferParent)
    {
        if (button != null || label == null || !Application.isPlaying)
        {
            return button;
        }

        Transform buttonTransform = label.transform;
        if (preferParent && label.transform.parent != null)
        {
            buttonTransform = label.transform.parent;
        }

        button = buttonTransform.GetComponent<Button>();
        if (button == null)
        {
            button = buttonTransform.gameObject.AddComponent<Button>();
        }

        Graphic targetGraphic = buttonTransform.GetComponent<Graphic>();
        button.targetGraphic = targetGraphic != null ? targetGraphic : label;
        button.transition = Selectable.Transition.None;
        label.raycastTarget = true;
        return button;
    }

    private void BindTabButtons()
    {
        if (primaryTabButton != null)
        {
            primaryTabButton.onClick.RemoveListener(ShowPrimaryStats);
            primaryTabButton.onClick.AddListener(ShowPrimaryStats);
        }

        if (secondaryTabButton != null)
        {
            secondaryTabButton.onClick.RemoveListener(ShowSecondaryStats);
            secondaryTabButton.onClick.AddListener(ShowSecondaryStats);
        }
    }

    private void UnbindTabButtons()
    {
        if (primaryTabButton != null)
        {
            primaryTabButton.onClick.RemoveListener(ShowPrimaryStats);
        }

        if (secondaryTabButton != null)
        {
            secondaryTabButton.onClick.RemoveListener(ShowSecondaryStats);
        }
    }

    private void UpdateTabVisuals()
    {
        if (primaryTabText != null)
        {
            primaryTabText.color = currentPage == StatsPage.Primary ? PositiveGreen : SoftWhite;
        }

        if (secondaryTabText != null)
        {
            secondaryTabText.color = currentPage == StatsPage.Secondary ? PositiveGreen : SoftWhite;
        }
    }

    private void EnsureRowCount(int count)
    {
        while (spawnedRows.Count < count)
        {
            PlayerStatRowView row = Instantiate(rowTemplate, rowsContainer);
            row.name = $"StatRow {spawnedRows.Count + 1:00}";
            spawnedRows.Add(row);
        }
    }

    private void CollectExistingRows()
    {
        if (rowsContainer == null)
        {
            return;
        }

        spawnedRows.Clear();
        PlayerStatRowView[] rows = rowsContainer.GetComponentsInChildren<PlayerStatRowView>(true);
        foreach (PlayerStatRowView row in rows)
        {
            if (row != null && row != rowTemplate && row.name != "RowTemplate")
            {
                spawnedRows.Add(row);
            }
        }
    }

    private void EnsureDefaultPreviewRows()
    {
        if (previewRows == null || previewRows.Count == 0)
        {
            previewRows = new List<PlayerStatDisplayEntry>
            {
                MakeWhite("level", "当前等级", "级", "23", new Color(0.82f, 0.92f, 1f, 1f)),
                MakeGreen("max_hp", "最大生命值", "心", "53", new Color(0.20f, 0.95f, 0.35f, 1f)),
                MakeGreen("hp_regeneration", "生命再生", "生", "0", new Color(0.35f, 1f, 0.35f, 1f)),
                MakeGreen("life_steal", "生命窃取", "窃", "5", new Color(0.95f, 0.25f, 0.32f, 1f)),
                MakeRed("damage", "伤害", "伤", "-17", new Color(1f, 0.20f, 0.25f, 1f)),
                MakeGreen("melee_damage", "近战伤害", "近", "5", new Color(0.95f, 0.88f, 0.45f, 1f)),
                MakeGreen("ranged_damage", "远程伤害", "远", "8", new Color(0.78f, 0.45f, 1f, 1f)),
                MakeGreen("elemental_damage", "元素伤害", "元", "2", new Color(1f, 0.58f, 0.35f, 1f)),
                MakeGreen("attack_speed", "攻击速度", "速", "23", new Color(0.90f, 0.90f, 0.90f, 1f)),
                MakeGreen("crit_chance", "暴击率", "暴", "59", new Color(1f, 0.20f, 0.25f, 1f)),
                MakeGreen("engineering", "工程学", "工", "8", new Color(0.35f, 0.95f, 1f, 1f)),
                MakeGreen("range", "范围", "范", "171", new Color(0.78f, 0.45f, 1f, 1f)),
                MakeGreen("armor", "护甲", "护", "3", new Color(0.98f, 0.88f, 0.20f, 1f)),
                MakeGreen("dodge", "闪避", "闪", "12", new Color(0.70f, 0.95f, 1f, 1f)),
                MakeGreen("speed", "速度", "移", "8", new Color(0.92f, 0.92f, 0.92f, 1f)),
                MakeGreen("luck", "幸运", "运", "22", new Color(0.98f, 0.98f, 0.98f, 1f)),
                MakeGreen("harvesting", "收获", "收", "18", new Color(1f, 0.90f, 0.48f, 1f)),
            };
        }

        if (secondaryPreviewRows != null && secondaryPreviewRows.Count > 0)
        {
            return;
        }

        Color secondaryColor = new Color(0.92f, 0.86f, 0.62f, 1f);
        secondaryPreviewRows = new List<PlayerStatDisplayEntry>
        {
            MakeWhite("consumable_heal", "消耗品治疗", "疗", "0", secondaryColor),
            MakeWhite("materials_healing", "材料治疗概率", "材", "0", secondaryColor),
            MakeWhite("xp_gain", "经验获取", "经", "0", secondaryColor),
            MakeWhite("pickup_range", "拾取范围", "拾", "0", secondaryColor),
            MakeWhite("items_price", "物品价格", "价", "0", secondaryColor),
            MakeWhite("explosion_damage", "爆炸伤害", "爆", "0", secondaryColor),
            MakeWhite("explosion_size", "爆炸范围", "域", "0", secondaryColor),
            MakeWhite("bounces", "弹射次数", "弹", "0", secondaryColor),
            MakeWhite("piercing", "贯穿次数", "贯", "0", secondaryColor),
            MakeWhite("piercing_damage", "贯穿伤害", "穿", "0", secondaryColor),
            MakeWhite("damage_against_bosses", "首领伤害", "首", "0", secondaryColor),
            MakeWhite("structure_attack_speed", "建筑攻击速度", "建", "0", secondaryColor),
            MakeWhite("structure_range", "建筑范围", "筑", "0", secondaryColor),
            MakeWhite("burning_speed", "燃烧速度", "燃", "0", secondaryColor),
            MakeWhite("burning_spread", "燃烧扩散", "烧", "0", secondaryColor),
            MakeWhite("knockback", "击退", "击", "0", secondaryColor),
            MakeWhite("double_material_chance", "双倍材料概率", "双", "0", secondaryColor),
            MakeWhite("free_rerolls", "免费刷新", "免", "0", secondaryColor),
            MakeWhite("trees", "树木数量", "树", "0", secondaryColor),
            MakeWhite("enemies", "敌人数量", "敌", "0", secondaryColor),
            MakeWhite("enemy_speed", "敌人速度", "怪", "0", secondaryColor),
            MakeWhite("reroll_price", "刷新价格", "刷", "0", secondaryColor),
        };
    }

    private static bool TrySetPreviewValue(
        IList<PlayerStatDisplayEntry> rows,
        string id,
        string valueText)
    {
        if (rows == null)
        {
            return false;
        }

        for (int index = 0; index < rows.Count; index++)
        {
            if (!string.Equals(rows[index].Id, id, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            PlayerStatDisplayEntry entry = rows[index];
            entry.ValueText = valueText;
            rows[index] = entry;
            return true;
        }

        return false;
    }

    private static PlayerStatDisplayEntry MakeGreen(
        string id,
        string displayName,
        string iconText,
        string valueText,
        Color iconColor)
    {
        return new PlayerStatDisplayEntry(id, displayName, iconText, valueText, iconColor);
    }

    private static PlayerStatDisplayEntry MakeRed(
        string id,
        string displayName,
        string iconText,
        string valueText,
        Color iconColor)
    {
        PlayerStatDisplayEntry entry = new PlayerStatDisplayEntry(id, displayName, iconText, valueText, iconColor);
        entry.NameColor = NegativeRed;
        entry.ValueColor = NegativeRed;
        return entry;
    }

    private static PlayerStatDisplayEntry MakeWhite(
        string id,
        string displayName,
        string iconText,
        string valueText,
        Color iconColor)
    {
        PlayerStatDisplayEntry entry = new PlayerStatDisplayEntry(id, displayName, iconText, valueText, iconColor, false);
        entry.NameColor = SoftWhite;
        entry.ValueColor = Color.white;
        return entry;
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
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (child.name != objectName)
                {
                    continue;
                }

                T component = child.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }

        return null;
    }
}
