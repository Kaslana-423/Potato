using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class RewardPanelSceneAssembler
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string BuildRequestPath = ".codex_reward_ui_request";
    private const string FontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/ShanHaiNiuNaiBoBoW-2 SDF.asset";

    private static readonly Color Ink = new Color(0.20f, 0.15f, 0.11f, 1f);
    private static readonly Color Paper = new Color(0.88f, 0.82f, 0.70f, 1f);
    private static readonly Color DarkPaper = new Color(0.63f, 0.54f, 0.40f, 1f);

    static RewardPanelSceneAssembler()
    {
        string requestPath = Path.GetFullPath(BuildRequestPath);
        if (!File.Exists(requestPath))
        {
            return;
        }

        File.Delete(requestPath);
        EditorApplication.delayCall += BuildRewardPanelsInSampleScene;
    }

    [MenuItem("Tools/Potato UI/Build Reward Panels In SampleScene")]
    public static void BuildRewardPanelsInSampleScene()
    {
        Scene previousActiveScene = SceneManager.GetActiveScene();
        Scene scene = SceneManager.GetSceneByPath(ScenePath);
        bool openedForBuild = !scene.IsValid() || !scene.isLoaded;
        if (openedForBuild)
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
        }

        SceneManager.SetActiveScene(scene);
        RemoveExistingRoot(scene, "LevelUpCanvas");
        RemoveExistingRoot(scene, "LootCrateRewardCanvas");

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null)
        {
            throw new MissingReferenceException($"Missing reward panel font: {FontPath}");
        }

        GameObject shopStatsPanel = FindSceneObjectByName(scene, "PlayerStatsPanel");
        if (shopStatsPanel == null)
        {
            throw new MissingReferenceException("SampleScene is missing the shop PlayerStatsPanel.");
        }

        BuildLevelUpCanvas(font, shopStatsPanel);
        BuildLootCrateCanvas(font);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
        {
            SceneManager.SetActiveScene(previousActiveScene);
        }
        if (openedForBuild)
        {
            EditorSceneManager.CloseScene(scene, true);
        }
        Debug.Log("Built scene-owned level-up and loot-crate reward panels in SampleScene.");
    }

    private static void BuildLevelUpCanvas(TMP_FontAsset font, GameObject shopStatsPanel)
    {
        GameObject canvasObject = CreateCanvas("LevelUpCanvas", 200);
        LevelUpRewardController controller = canvasObject.AddComponent<LevelUpRewardController>();
        GameObject window = CreateWindow("LevelUpWindow", canvasObject.transform);

        GameObject panel = CreateUiObject("Panel", window.transform);
        SetRect(panel.GetComponent<RectTransform>(), new Vector2(0.0417f, 0.1204f), new Vector2(0.9583f, 0.8796f));
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = Paper;

        TMP_Text title = CreateText("Title", panel.transform, font, 44f, FontStyles.Bold, Ink);
        SetRect(title.rectTransform, new Vector2(0.04f, 0.87f), new Vector2(0.72f, 0.98f));
        title.text = "升级奖励";
        title.alignment = TextAlignmentOptions.Center;

        TMP_Text pending = CreateText("Pending", panel.transform, font, 23f, FontStyles.Normal, Ink);
        SetRect(pending.rectTransform, new Vector2(0.04f, 0.79f), new Vector2(0.72f, 0.87f));
        pending.text = "选择一项属性强化";
        pending.alignment = TextAlignmentOptions.Center;

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

        Color[] previewColors =
        {
            new Color(0.83f, 0.77f, 0.63f, 1f),
            new Color(0.78f, 0.84f, 0.68f, 1f),
            new Color(0.72f, 0.80f, 0.86f, 1f),
            new Color(0.85f, 0.72f, 0.83f, 1f),
        };
        for (int index = 0; index < 4; index++)
        {
            BuildUpgradeOption(optionsRoot.transform, font, previewColors[index]);
        }

        Button reroll = CreateButton("RerollButton", panel.transform, font, "重随", DarkPaper, Ink, out _);
        SetRect((RectTransform)reroll.transform, new Vector2(0.3001f, 0.04f), new Vector2(0.4649f, 0.14f));

        GameObject divider = CreateUiObject("StatsDivider", panel.transform);
        SetRect(divider.GetComponent<RectTransform>(), new Vector2(0.752f, 0.04f), new Vector2(0.754f, 0.96f));
        divider.AddComponent<Image>().color = new Color(0.38f, 0.30f, 0.21f, 1f);

        GameObject statsPanel = Object.Instantiate(shopStatsPanel);
        statsPanel.name = "UpgradeStatsPanel";
        statsPanel.transform.SetParent(panel.transform, false);
        SetRect((RectTransform)statsPanel.transform, new Vector2(0.765f, 0.035f), new Vector2(0.985f, 0.965f));
        statsPanel.SetActive(true);

        ApplyFontRecursively(canvasObject, font);

        controller.AutoBindReferences();
        EditorUtility.SetDirty(controller);
        window.SetActive(false);
    }

    private static void BuildUpgradeOption(Transform parent, TMP_FontAsset font, Color color)
    {
        GameObject card = CreateUiObject("UpgradeOption", parent);
        Image background = card.AddComponent<Image>();
        background.color = color;
        Button button = card.AddComponent<Button>();
        button.targetGraphic = background;

        TMP_Text tier = CreateText("Tier", card.transform, font, 22f, FontStyles.Bold, Ink);
        SetRect(tier.rectTransform, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.95f));
        tier.text = "等级 I";
        tier.alignment = TextAlignmentOptions.Center;

        TMP_Text name = CreateText("Name", card.transform, font, 31f, FontStyles.Bold, Ink);
        SetRect(name.rectTransform, new Vector2(0.08f, 0.5f), new Vector2(0.92f, 0.78f));
        name.text = "属性";
        name.alignment = TextAlignmentOptions.Center;

        TMP_Text value = CreateText("Value", card.transform, font, 46f, FontStyles.Bold, new Color(0.16f, 0.48f, 0.20f, 1f));
        SetRect(value.rectTransform, new Vector2(0.08f, 0.25f), new Vector2(0.92f, 0.52f));
        value.text = "+1";
        value.alignment = TextAlignmentOptions.Center;

        TMP_Text current = CreateText("Current", card.transform, font, 19f, FontStyles.Normal, Ink);
        SetRect(current.rectTransform, new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.23f));
        current.text = "当前 0 → 1";
        current.alignment = TextAlignmentOptions.Center;
    }

    private static void BuildLootCrateCanvas(TMP_FontAsset font)
    {
        GameObject canvasObject = CreateCanvas("LootCrateRewardCanvas", 190);
        LootCrateRewardController controller = canvasObject.AddComponent<LootCrateRewardController>();
        GameObject window = CreateWindow("LootCrateRewardWindow", canvasObject.transform);

        GameObject panel = CreateUiObject("Panel", window.transform);
        SetRect(panel.GetComponent<RectTransform>(), new Vector2(0.2552f, 0.1481f), new Vector2(0.7448f, 0.8519f));
        panel.AddComponent<Image>().color = Paper;

        TMP_Text header = CreateText("Header", panel.transform, font, 42f, FontStyles.Bold, Ink);
        SetRect(header.rectTransform, new Vector2(0.05f, 0.89f), new Vector2(0.95f, 0.98f));
        header.text = "战利品箱";
        header.alignment = TextAlignmentOptions.Center;

        TMP_Text pending = CreateText("Pending", panel.transform, font, 21f, FontStyles.Normal, Ink);
        SetRect(pending.rectTransform, new Vector2(0.05f, 0.83f), new Vector2(0.95f, 0.89f));
        pending.text = "最后一个战利品箱";
        pending.alignment = TextAlignmentOptions.Center;

        GameObject iconObject = CreateUiObject("ItemIcon", panel.transform);
        SetRect(iconObject.GetComponent<RectTransform>(), new Vector2(0.08f, 0.52f), new Vector2(0.34f, 0.82f));
        Image itemIcon = iconObject.AddComponent<Image>();
        itemIcon.preserveAspect = true;
        itemIcon.raycastTarget = false;
        itemIcon.color = Color.clear;

        TMP_Text placeholder = CreateText("IconPlaceholder", iconObject.transform, font, 96f, FontStyles.Bold, Ink);
        Stretch(placeholder.rectTransform);
        placeholder.text = "?";
        placeholder.alignment = TextAlignmentOptions.Center;

        TMP_Text itemName = CreateText("ItemName", panel.transform, font, 36f, FontStyles.Bold, Ink);
        SetRect(itemName.rectTransform, new Vector2(0.39f, 0.7f), new Vector2(0.92f, 0.82f));
        itemName.text = "获得道具";
        itemName.alignment = TextAlignmentOptions.Left;

        TMP_Text rarity = CreateText("Rarity", panel.transform, font, 23f, FontStyles.Bold, Ink);
        SetRect(rarity.rectTransform, new Vector2(0.39f, 0.62f), new Vector2(0.92f, 0.7f));
        rarity.text = "普通";
        rarity.alignment = TextAlignmentOptions.Left;

        TMP_Text details = CreateText("Details", panel.transform, font, 23f, FontStyles.Normal, Ink);
        SetRect(details.rectTransform, new Vector2(0.08f, 0.27f), new Vector2(0.92f, 0.59f));
        details.text = "道具效果说明";
        details.alignment = TextAlignmentOptions.TopLeft;
        details.enableAutoSizing = true;
        details.fontSizeMin = 15f;
        details.fontSizeMax = 100f;
        details.overflowMode = TextOverflowModes.Ellipsis;

        Button take = CreateButton("TakeButton", panel.transform, font, "收下", new Color(0.55f, 0.70f, 0.43f, 1f), Ink, out _);
        SetRect((RectTransform)take.transform, new Vector2(0.12f, 0.09f), new Vector2(0.46f, 0.21f));
        Button recycle = CreateButton("RecycleButton", panel.transform, font, "回收", DarkPaper, Ink, out _);
        SetRect((RectTransform)recycle.transform, new Vector2(0.54f, 0.09f), new Vector2(0.88f, 0.21f));

        TMP_Text error = CreateText("Error", panel.transform, font, 18f, FontStyles.Bold, new Color(0.66f, 0.14f, 0.12f, 1f));
        SetRect(error.rectTransform, new Vector2(0.08f, 0.015f), new Vector2(0.92f, 0.075f));
        error.alignment = TextAlignmentOptions.Center;

        ApplyFontRecursively(canvasObject, font);

        controller.AutoBindReferences();
        EditorUtility.SetDirty(controller);
        window.SetActive(false);
    }

    private static GameObject CreateCanvas(string objectName, int sortingOrder)
    {
        GameObject canvasObject = new GameObject(objectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.layer = 5;
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        ResponsiveUiLayout.ConfigureCanvasScaler(canvasObject.GetComponent<CanvasScaler>());
        return canvasObject;
    }

    private static GameObject CreateWindow(string objectName, Transform parent)
    {
        GameObject window = CreateUiObject(objectName, parent);
        Stretch(window.GetComponent<RectTransform>());
        Image dimmer = window.AddComponent<Image>();
        dimmer.color = new Color(0.05f, 0.04f, 0.035f, 0.82f);
        return window;
    }

    private static Button CreateButton(string objectName, Transform parent, TMP_FontAsset font, string label,
        Color backgroundColor, Color textColor, out TMP_Text labelText)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        Image image = buttonObject.AddComponent<Image>();
        image.color = backgroundColor;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        labelText = CreateText("Text", buttonObject.transform, font, 27f, FontStyles.Bold, textColor);
        Stretch(labelText.rectTransform);
        labelText.text = label;
        labelText.alignment = TextAlignmentOptions.Center;
        return button;
    }

    private static TMP_Text CreateText(string objectName, Transform parent, TMP_FontAsset font, float fontSize,
        FontStyles style, Color color)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.fontSize = fontSize;
        text.fontSizeMax = 100f;
        text.fontStyle = style;
        text.color = color;
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        return text;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject result = new GameObject(objectName, typeof(RectTransform));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Stretch(RectTransform rect) => SetRect(rect, Vector2.zero, Vector2.one);

    private static void ApplyFontRecursively(GameObject root, TMP_FontAsset font)
    {
        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            text.font = font;
            text.fontSizeMax = 100f;
            EditorUtility.SetDirty(text);
        }
    }

    private static GameObject FindSceneObjectByName(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == objectName)
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }

    private static void RemoveExistingRoot(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == objectName)
            {
                Object.DestroyImmediate(root);
                return;
            }
        }
    }
}
