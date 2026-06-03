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
    public static readonly Color PositiveGreen = new Color(0.05f, 1f, 0.12f, 1f);
    public static readonly Color NegativeRed = new Color(1f, 0.18f, 0.18f, 1f);
    public static readonly Color SoftWhite = new Color(0.92f, 0.92f, 0.92f, 1f);

    [Header("Panel References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text primaryTabText;
    [SerializeField] private TMP_Text secondaryTabText;
    [SerializeField] private Transform rowsContainer;
    [SerializeField] private PlayerStatRowView rowTemplate;

    [Header("Preview Data")]
    [SerializeField] private bool renderPreviewOnStart = true;
    [SerializeField] private bool bindPlayerStatsOnStart = true;
    [SerializeField] private List<PlayerStatDisplayEntry> previewRows = new List<PlayerStatDisplayEntry>();

    private readonly List<PlayerStatRowView> spawnedRows = new List<PlayerStatRowView>();
    private PlayerStats boundPlayerStats;

    private void Awake()
    {
        AutoBindReferences();
        CollectExistingRows();
        EnsureDefaultPreviewRows();
    }

    private void Start()
    {
        if (bindPlayerStatsOnStart)
        {
            BindPlayerStats(PlayerStats.Instance);
        }

        if (boundPlayerStats == null && renderPreviewOnStart)
        {
            SetRows(previewRows);
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

        if (rowsContainer == null)
        {
            rowsContainer = FindDescendant("RowsContainer", "Rows");
        }

        if (rowTemplate == null)
        {
            rowTemplate = FindComponent<PlayerStatRowView>("RowTemplate");
        }
    }

    public void SetRows(IReadOnlyList<PlayerStatDisplayEntry> rows)
    {
        AutoBindReferences();

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
        EnsureRowCount(rows.Count);

        for (int index = 0; index < spawnedRows.Count; index++)
        {
            bool visible = index < rows.Count;
            spawnedRows[index].gameObject.SetActive(visible);
            if (visible)
            {
                spawnedRows[index].Bind(rows[index]);
            }
        }
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

        for (int index = 0; index < previewRows.Count; index++)
        {
            if (!string.Equals(previewRows[index].Id, id, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            PlayerStatDisplayEntry entry = previewRows[index];
            entry.ValueText = valueText;
            previewRows[index] = entry;
            break;
        }

        SetRows(previewRows);
    }

    public void RefreshFromPlayerStats()
    {
        if (boundPlayerStats != null)
        {
            SetRows(boundPlayerStats.BuildDisplayEntries());
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
        if (previewRows != null && previewRows.Count > 0)
        {
            return;
        }

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
        Transform child = FindDescendant(names);
        return child != null ? child.GetComponent<T>() : null;
    }
}
