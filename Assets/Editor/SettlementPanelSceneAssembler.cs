using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SettlementPanelSceneAssembler
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string FontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/ShanHaiNiuNaiBoBoW-2 SDF.asset";

    private static readonly Color Ink = new Color(0.20f, 0.15f, 0.11f, 1f);
    private static readonly Color Paper = new Color(0.88f, 0.82f, 0.70f, 1f);
    private static readonly Color DarkPaper = new Color(0.63f, 0.54f, 0.40f, 1f);

    [MenuItem("Tools/Potato UI/Build Settlement Panel In SampleScene")]
    public static void BuildSettlementPanelInSampleScene()
    {
        Scene previousActiveScene = SceneManager.GetActiveScene();
        Scene scene = SceneManager.GetSceneByPath(ScenePath);
        bool openedForBuild = !scene.IsValid() || !scene.isLoaded;
        if (openedForBuild)
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
        }

        SceneManager.SetActiveScene(scene);
        RemoveExistingRoot(scene, "GameSettlementCanvas");

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null)
        {
            throw new MissingReferenceException($"Missing settlement panel font: {FontPath}");
        }

        GameObject canvasObject = CreateCanvas("GameSettlementCanvas", 300);
        GameRunSettlementController controller = canvasObject.AddComponent<GameRunSettlementController>();
        GameObject window = CreateWindow("SettlementWindow", canvasObject.transform);

        GameObject panel = CreateUiObject("Panel", window.transform);
        SetRect(panel.GetComponent<RectTransform>(), new Vector2(0.3021f, 0.1481f), new Vector2(0.6979f, 0.8519f));
        panel.AddComponent<Image>().color = Paper;

        TMP_Text title = CreateText("Title", panel.transform, font, 62f, FontStyles.Bold, Ink);
        SetRect(title.rectTransform, new Vector2(0.07f, 0.81f), new Vector2(0.93f, 0.96f));
        title.text = "战斗结算";
        title.alignment = TextAlignmentOptions.Center;

        TMP_Text outcome = CreateText("Outcome", panel.transform, font, 28f, FontStyles.Bold, Ink);
        SetRect(outcome.rectTransform, new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.82f));
        outcome.text = "战斗结果";
        outcome.alignment = TextAlignmentOptions.Center;

        TMP_Text stats = CreateText("Stats", panel.transform, font, 28f, FontStyles.Normal, Ink);
        SetRect(stats.rectTransform, new Vector2(0.2f, 0.28f), new Vector2(0.8f, 0.68f));
        stats.text = "角色等级    Lv.1\n击杀敌人    0\n持有材料    0\n保留材料    0\n游戏时间    00:00";
        stats.alignment = TextAlignmentOptions.MidlineLeft;
        stats.lineSpacing = 18f;

        Button restart = CreateButton("RestartButton", panel.transform, font, "重新开始", new Color(0.55f, 0.70f, 0.43f, 1f));
        SetRect((RectTransform)restart.transform, new Vector2(0.1f, 0.08f), new Vector2(0.47f, 0.21f));
        Button mainMenu = CreateButton("MainMenuButton", panel.transform, font, "返回主菜单", DarkPaper);
        SetRect((RectTransform)mainMenu.transform, new Vector2(0.53f, 0.08f), new Vector2(0.9f, 0.21f));

        controller.AutoBindReferences();
        EditorUtility.SetDirty(controller);
        window.SetActive(false);

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
        Debug.Log("Built scene-owned victory and defeat settlement UI in SampleScene.");
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
        window.AddComponent<CanvasGroup>();
        Image dimmer = window.AddComponent<Image>();
        dimmer.color = new Color(0.05f, 0.04f, 0.035f, 0.82f);
        return window;
    }

    private static Button CreateButton(string objectName, Transform parent, TMP_FontAsset font, string label, Color backgroundColor)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        Image image = buttonObject.AddComponent<Image>();
        image.color = backgroundColor;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        TMP_Text labelText = CreateText("Text", buttonObject.transform, font, 27f, FontStyles.Bold, Ink);
        Stretch(labelText.rectTransform);
        labelText.text = label;
        labelText.alignment = TextAlignmentOptions.Center;
        return button;
    }

    private static TMP_Text CreateText(string objectName, Transform parent, TMP_FontAsset font, float fontSize, FontStyles style, Color color)
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
