using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class ShopItemPrefabCreator
{
    private const string PrefabDirectory = "Assets/Prefebs";
    private const string PrefabPath = PrefabDirectory + "/ShopItem.prefab";
    private const string ChineseFontPath =
        "Assets/TextMesh Pro/Resources/Fonts & Materials/SmileySans-Oblique SDF.asset";

    [MenuItem("Tools/Potato Shop/Create ShopItem Prefab Template")]
    public static void CreateShopItemPrefabTemplate()
    {
        EnsureDirectoryExists();

        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null
            && !EditorUtility.DisplayDialog(
                "Potato Shop",
                $"{PrefabPath} already exists. Replace it with a new template?",
                "Replace",
                "Cancel"))
        {
            return;
        }

        GameObject root = CreateUiObject("ShopItem", null, Vector2.zero, new Vector2(450f, 600f));
        try
        {
            Image background = root.AddComponent<Image>();
            background.color = new Color(0.035f, 0.035f, 0.035f, 0.98f);

            Button inspectButton = root.AddComponent<Button>();
            inspectButton.targetGraphic = background;

            GameObject iconPanel = CreateUiObject(
                "IconPanel",
                root.transform,
                new Vector2(-145f, 210f),
                new Vector2(132f, 132f));
            Image iconPanelImage = iconPanel.AddComponent<Image>();
            iconPanelImage.color = new Color(0.14f, 0.14f, 0.14f, 1f);

            GameObject icon = CreateUiObject("Icon", iconPanel.transform, Vector2.zero, new Vector2(118f, 118f));
            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;

            CreateText(
                "IconPlaceholder",
                icon.transform,
                "道",
                Vector2.zero,
                new Vector2(104f, 104f),
                48,
                TextAlignmentOptions.Center,
                Color.white);

            CreateText(
                "NameText",
                root.transform,
                "道具名称",
                new Vector2(65f, 252f),
                new Vector2(250f, 48f),
                30,
                TextAlignmentOptions.Left,
                Color.white);

            CreateText(
                "KindText",
                root.transform,
                "道具",
                new Vector2(65f, 210f),
                new Vector2(250f, 34f),
                21,
                TextAlignmentOptions.Left,
                new Color(0.94f, 0.87f, 0.62f, 1f));

            CreateText(
                "LimitText",
                root.transform,
                "限制 (0/1)",
                new Vector2(65f, 173f),
                new Vector2(250f, 30f),
                18,
                TextAlignmentOptions.Left,
                new Color(0.94f, 0.87f, 0.62f, 1f));

            CreateText(
                "StatsText",
                root.transform,
                "<color=#55E875>+3</color> 暴击率\n<color=#FF6868>-2</color> 闪避",
                new Vector2(0f, 48f),
                new Vector2(410f, 180f),
                20,
                TextAlignmentOptions.TopLeft,
                new Color(0.93f, 0.93f, 0.93f, 1f));

            CreateText(
                "DescriptionText",
                root.transform,
                "• 道具效果说明",
                new Vector2(0f, -100f),
                new Vector2(410f, 96f),
                18,
                TextAlignmentOptions.TopLeft,
                new Color(0.93f, 0.93f, 0.93f, 1f));

            GameObject pricePanel = CreateUiObject(
                "PricePanel",
                root.transform,
                new Vector2(0f, -250f),
                new Vector2(180f, 72f));
            Image pricePanelImage = pricePanel.AddComponent<Image>();
            pricePanelImage.color = new Color(0.14f, 0.14f, 0.14f, 1f);

            CreateText(
                "PriceText",
                pricePanel.transform,
                "<color=#90E65A>30</color> 材料",
                Vector2.zero,
                new Vector2(180f, 72f),
                30,
                TextAlignmentOptions.Center,
                Color.white);

            ShopOfferView view = root.AddComponent<ShopOfferView>();
            view.AutoBindReferences();
            view.EnsureLockButton();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = prefab;

            Debug.Log($"Created reusable shop item prefab at {PrefabPath}");
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
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
        Color color)
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
        text.alignment = alignment;
        text.color = color;
        text.enableWordWrapping = true;
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
