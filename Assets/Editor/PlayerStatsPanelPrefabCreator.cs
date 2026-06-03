using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class PlayerStatsPanelPrefabCreator
{
    private const string PrefabDirectory = "Assets/Prefebs";
    private const string PrefabPath = PrefabDirectory + "/PlayerStatsPanel.prefab";
    private const string ChineseFontPath =
        "Assets/TextMesh Pro/Resources/Fonts & Materials/SmileySans-Oblique SDF.asset";

    [MenuItem("Tools/Potato UI/Create Player Stats Panel Prefab")]
    public static void CreatePlayerStatsPanelPrefab()
    {
        EnsureDirectoryExists();

        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null
            && !EditorUtility.DisplayDialog(
                "Potato UI",
                $"{PrefabPath} already exists. Replace it with a new stats panel?",
                "Replace",
                "Cancel"))
        {
            return;
        }

        GameObject root = CreateUiObject("PlayerStatsPanel", null, Vector2.zero, new Vector2(280f, 526f));
        try
        {
            Image background = root.AddComponent<Image>();
            background.color = new Color(0.095f, 0.088f, 0.082f, 0.94f);

            CreateText(
                "TitleText",
                root.transform,
                "属性",
                new Vector2(0f, 236f),
                new Vector2(220f, 44f),
                30,
                TextAlignmentOptions.Center,
                Color.white,
                FontStyles.Bold);

            CreateText(
                "PrimaryTabText",
                root.transform,
                "主要",
                new Vector2(-60f, 192f),
                new Vector2(100f, 34f),
                20,
                TextAlignmentOptions.Center,
                Color.white,
                FontStyles.Bold);

            GameObject secondaryTab = CreateUiObject(
                "SecondaryTab",
                root.transform,
                new Vector2(62f, 192f),
                new Vector2(120f, 34f));
            Image secondaryTabBackground = secondaryTab.AddComponent<Image>();
            secondaryTabBackground.color = new Color(0.02f, 0.02f, 0.02f, 0.96f);
            CreateText(
                "SecondaryTabText",
                secondaryTab.transform,
                "次要",
                Vector2.zero,
                new Vector2(120f, 34f),
                20,
                TextAlignmentOptions.Center,
                Color.white,
                FontStyles.Bold);

            GameObject rowsContainer = CreateUiObject(
                "RowsContainer",
                root.transform,
                new Vector2(0f, -43f),
                new Vector2(244f, 430f));
            VerticalLayoutGroup layout = rowsContainer.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = 2f;

            PlayerStatRowView rowTemplate = CreateRow("RowTemplate", rowsContainer.transform);
            rowTemplate.gameObject.SetActive(false);

            foreach (PlayerStatDisplayEntry entry in CreatePreviewRows())
            {
                PlayerStatRowView row = CreateRow($"StatRow {rowsContainer.transform.childCount:00}", rowsContainer.transform);
                row.Bind(entry);
            }

            PlayerStatsPanelView panel = root.AddComponent<PlayerStatsPanelView>();
            panel.AutoBindReferences();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = prefab;

            Debug.Log($"Created player stats panel prefab at {PrefabPath}");
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    private static PlayerStatRowView CreateRow(string name, Transform parent)
    {
        GameObject rowObject = CreateUiObject(name, parent, Vector2.zero, new Vector2(244f, 22f));

        CreateText(
            "IconText",
            rowObject.transform,
            "心",
            new Vector2(-108f, 0f),
            new Vector2(26f, 22f),
            18,
            TextAlignmentOptions.Center,
            Color.white,
            FontStyles.Bold);

        GameObject iconImageObject = CreateUiObject(
            "IconImage",
            rowObject.transform,
            new Vector2(-108f, 0f),
            new Vector2(20f, 20f));
        Image iconImage = iconImageObject.AddComponent<Image>();
        iconImage.color = Color.white;
        iconImage.preserveAspect = true;
        iconImageObject.SetActive(false);

        CreateText(
            "NameText",
            rowObject.transform,
            "最大生命值",
            new Vector2(-30f, 0f),
            new Vector2(146f, 22f),
            16,
            TextAlignmentOptions.Left,
            PlayerStatsPanelView.PositiveGreen,
            FontStyles.Normal);

        CreateText(
            "ValueText",
            rowObject.transform,
            "53",
            new Vector2(102f, 0f),
            new Vector2(42f, 22f),
            16,
            TextAlignmentOptions.Right,
            PlayerStatsPanelView.PositiveGreen,
            FontStyles.Bold);

        PlayerStatRowView row = rowObject.AddComponent<PlayerStatRowView>();
        row.AutoBindReferences();
        return row;
    }

    private static IEnumerable<PlayerStatDisplayEntry> CreatePreviewRows()
    {
        yield return MakeWhite("level", "当前等级", "级", "23", new Color(0.82f, 0.92f, 1f, 1f));
        yield return MakeGreen("max_hp", "最大生命值", "心", "53", new Color(0.20f, 0.95f, 0.35f, 1f));
        yield return MakeGreen("hp_regeneration", "生命再生", "生", "0", new Color(0.35f, 1f, 0.35f, 1f));
        yield return MakeGreen("life_steal", "生命窃取", "窃", "5", new Color(0.95f, 0.25f, 0.32f, 1f));
        yield return MakeRed("damage", "伤害", "伤", "-17", new Color(1f, 0.20f, 0.25f, 1f));
        yield return MakeGreen("melee_damage", "近战伤害", "近", "5", new Color(0.95f, 0.88f, 0.45f, 1f));
        yield return MakeGreen("ranged_damage", "远程伤害", "远", "8", new Color(0.78f, 0.45f, 1f, 1f));
        yield return MakeGreen("elemental_damage", "元素伤害", "元", "2", new Color(1f, 0.58f, 0.35f, 1f));
        yield return MakeGreen("attack_speed", "攻击速度", "速", "23", new Color(0.90f, 0.90f, 0.90f, 1f));
        yield return MakeGreen("crit_chance", "暴击率", "暴", "59", new Color(1f, 0.20f, 0.25f, 1f));
        yield return MakeGreen("engineering", "工程学", "工", "8", new Color(0.35f, 0.95f, 1f, 1f));
        yield return MakeGreen("range", "范围", "范", "171", new Color(0.78f, 0.45f, 1f, 1f));
        yield return MakeGreen("armor", "护甲", "护", "3", new Color(0.98f, 0.88f, 0.20f, 1f));
        yield return MakeGreen("dodge", "闪避", "闪", "12", new Color(0.70f, 0.95f, 1f, 1f));
        yield return MakeGreen("speed", "速度", "移", "8", new Color(0.92f, 0.92f, 0.92f, 1f));
        yield return MakeGreen("luck", "幸运", "运", "22", new Color(0.98f, 0.98f, 0.98f, 1f));
        yield return MakeGreen("harvesting", "收获", "收", "18", new Color(1f, 0.90f, 0.48f, 1f));
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
        entry.NameColor = PlayerStatsPanelView.NegativeRed;
        entry.ValueColor = PlayerStatsPanelView.NegativeRed;
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
        entry.NameColor = PlayerStatsPanelView.SoftWhite;
        entry.ValueColor = Color.white;
        return entry;
    }

    private static GameObject CreateUiObject(
        string name,
        Transform parent,
        Vector2 anchoredPosition,
        Vector2 size)
    {
        var uiObject = new GameObject(name, typeof(RectTransform));
        uiObject.layer = 5;
        uiObject.transform.SetParent(parent, false);

        RectTransform rect = uiObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        return uiObject;
    }

    private static TMP_Text CreateText(
        string name,
        Transform parent,
        string value,
        Vector2 anchoredPosition,
        Vector2 size,
        int fontSize,
        TextAlignmentOptions alignment,
        Color color,
        FontStyles fontStyle)
    {
        GameObject textObject = CreateUiObject(name, parent, anchoredPosition, size);
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        TMP_FontAsset chineseFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ChineseFontPath);
        if (chineseFont != null)
        {
            text.font = chineseFont;
        }

        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = color;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Truncate;
        return text;
    }

    private static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(PrefabDirectory))
        {
            Directory.CreateDirectory(PrefabDirectory);
        }
    }
}
