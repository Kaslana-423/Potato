using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MainMenuSceneCreator
{
    private const string SceneDirectory = "Assets/Scenes";
    private const string ScenePath = SceneDirectory + "/MainMenu.unity";
    private const string GameplayScenePath = SceneDirectory + "/SampleScene.unity";
    private const string ChineseFontPath =
        "Assets/TextMesh Pro/Resources/Fonts & Materials/SmileySans-Oblique SDF.asset";

    private static readonly Color Background = new Color(0.025f, 0.035f, 0.055f, 1f);
    private static readonly Color Panel = new Color(0.055f, 0.075f, 0.105f, 0.97f);
    private static readonly Color PanelLight = new Color(0.085f, 0.115f, 0.15f, 1f);
    private static readonly Color Accent = new Color(0.52f, 0.88f, 0.36f, 1f);
    private static readonly Color Danger = new Color(0.82f, 0.26f, 0.24f, 1f);
    private static readonly Color Muted = new Color(0.63f, 0.69f, 0.76f, 1f);

    [InitializeOnLoadMethod]
    private static void ScheduleMissingSceneGeneration()
    {
        if (!File.Exists(ScenePath))
        {
            EditorApplication.delayCall += CreateMissingMainMenuScene;
        }
    }

    private static void CreateMissingMainMenuScene()
    {
        if (File.Exists(ScenePath))
        {
            return;
        }

        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.delayCall += CreateMissingMainMenuScene;
            return;
        }

        CreateMainMenuScene();
    }

    [MenuItem("Tools/Potato UI/Create Main Menu Scene")]
    public static void CreateMainMenuScene()
    {
        Directory.CreateDirectory(SceneDirectory);

        Scene previousScene = SceneManager.GetActiveScene();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);

        try
        {
            BuildScene(scene);
        }
        finally
        {
            if (previousScene.IsValid() && previousScene.isLoaded)
            {
                SceneManager.SetActiveScene(previousScene);
            }

            if (scene.IsValid() && scene.isLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }

    private static void BuildScene(Scene scene)
    {
        CreateCamera();

        Canvas canvas = CreateCanvas();
        Transform root = canvas.transform;

        CreateImage("Background", root, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Background);
        CreateImage(
            "TopAccent",
            root,
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            Vector2.zero,
            new Vector2(0f, 8f),
            Accent);

        CreateImage(
            "LeftGlow",
            root,
            new Vector2(0f, 0f),
            new Vector2(0.43f, 1f),
            Vector2.zero,
            Vector2.zero,
            new Color(0.075f, 0.13f, 0.11f, 0.44f));

        GameObject controllerObject = CreateUiObject(
            "MainMenuController",
            root,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero);

        CreateBrandArea(controllerObject.transform);
        MainActions actions = CreateMainActions(controllerObject.transform);
        SettingsReferences settings = CreateSettingsPanel(controllerObject.transform);
        ConfirmationReferences confirmation = CreateConfirmationPanel(controllerObject.transform);

        MainMenuFlowView navigationView = controllerObject.AddComponent<MainMenuFlowView>();
        TMP_FontAsset navigationFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ChineseFontPath);
        navigationView.EnsurePages(actions.panel, navigationFont);
        UIRouter router = controllerObject.AddComponent<UIRouter>();
        UIScreen titleScreen = ConfigureScreen(
            navigationView.TitlePanel,
            UIRoute.Title,
            navigationView.TitleContinueButton);
        UIScreen saveSelectScreen = ConfigureScreen(
            navigationView.SaveSelectPanel,
            UIRoute.SaveSelect,
            navigationView.FirstSaveSlotButton);
        UIScreen mainMenuScreen = ConfigureScreen(actions.panel, UIRoute.MainMenu, actions.start);
        UIScreen characterSelectScreen = ConfigureScreen(
            navigationView.CharacterSelectPanel,
            UIRoute.CharacterSelect,
            navigationView.DefaultCharacterButton);
        UIScreen settingsScreen = ConfigureScreen(settings.panel, UIRoute.Settings, settings.volume);

        MainMenuController controller = controllerObject.AddComponent<MainMenuController>();
        controller.Configure(
            actions.start,
            actions.continueGame,
            actions.abandon,
            actions.settings,
            actions.exit,
            actions.sessionStatus,
            settings.panel,
            settings.volume,
            settings.volumeValue,
            settings.fullscreen,
            settings.back,
            confirmation.panel,
            confirmation.message,
            confirmation.confirm,
            confirmation.cancel);
        controller.ConfigureNavigationReferences(
            actions.panel,
            navigationView,
            router,
            titleScreen,
            saveSelectScreen,
            mainMenuScreen,
            characterSelectScreen,
            settingsScreen);

        settings.panel.SetActive(false);
        confirmation.panel.SetActive(false);
        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, ScenePath);
        ConfigureBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);

        Debug.Log($"Created main menu scene at {ScenePath}");
    }

    private static void CreateBrandArea(Transform parent)
    {
        GameObject brand = CreateUiObject(
            "BrandArea",
            parent,
            new Vector2(0.07f, 0.12f),
            new Vector2(0.55f, 0.88f),
            Vector2.zero,
            Vector2.zero);

        CreateText(
            "EyebrowText",
            brand.transform,
            "TOP-DOWN SURVIVAL PROTOTYPE",
            new Vector2(0f, 0.78f),
            new Vector2(1f, 0.9f),
            new Vector2(0f, 0f),
            new Vector2(-20f, 0f),
            22f,
            TextAlignmentOptions.Left,
            Accent,
            FontStyles.Bold);

        CreateText(
            "GameTitleText",
            brand.transform,
            "POTATO\nSURVIVOR",
            new Vector2(0f, 0.35f),
            new Vector2(1f, 0.79f),
            Vector2.zero,
            new Vector2(-10f, 0f),
            92f,
            TextAlignmentOptions.Left,
            Color.white,
            FontStyles.Bold);

        CreateImage(
            "TitleDivider",
            brand.transform,
            new Vector2(0f, 0.31f),
            new Vector2(0.58f, 0.31f),
            Vector2.zero,
            new Vector2(0f, 4f),
            Accent);

        CreateText(
            "DescriptionText",
            brand.transform,
            "撑过一波又一波敌人。\n收集材料，构筑武器，在商店中强化自己。",
            new Vector2(0f, 0.09f),
            new Vector2(0.82f, 0.29f),
            Vector2.zero,
            Vector2.zero,
            30f,
            TextAlignmentOptions.TopLeft,
            Muted,
            FontStyles.Normal);

        CreateText(
            "VersionText",
            brand.transform,
            "PROTOTYPE  ·  2026",
            new Vector2(0f, 0f),
            new Vector2(0.6f, 0.08f),
            Vector2.zero,
            Vector2.zero,
            18f,
            TextAlignmentOptions.BottomLeft,
            new Color(0.4f, 0.47f, 0.55f, 1f),
            FontStyles.Normal);
    }

    private static MainActions CreateMainActions(Transform parent)
    {
        GameObject panelObject = CreateUiObject(
            "MainActionsPanel",
            parent,
            new Vector2(0.62f, 0.12f),
            new Vector2(0.93f, 0.88f),
            Vector2.zero,
            Vector2.zero);
        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = Panel;

        VerticalLayoutGroup layout = panelObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(52, 52, 48, 48);
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        TMP_Text heading = CreateLayoutText("MenuHeadingText", panelObject.transform, "主菜单", 40f, Color.white, 62f);
        heading.alignment = TextAlignmentOptions.Left;
        heading.fontStyle = FontStyles.Bold;

        TMP_Text status = CreateLayoutText(
            "SessionStatusText",
            panelObject.transform,
            "当前没有进行中的游戏",
            20f,
            Muted,
            42f);
        status.alignment = TextAlignmentOptions.Left;

        AddSpacer(panelObject.transform, 6f);
        Button start = CreateMenuButton("StartGameButton", panelObject.transform, "开始游戏", Accent, Background);
        Button continueGame = CreateMenuButton("ContinueGameButton", panelObject.transform, "继续游戏", PanelLight, Color.white);
        Button abandon = CreateMenuButton("AbandonRunButton", panelObject.transform, "放弃当前游戏", PanelLight, Danger);
        AddSpacer(panelObject.transform, 8f);
        Button settings = CreateMenuButton("SettingsButton", panelObject.transform, "设置", PanelLight, Color.white);
        Button exit = CreateMenuButton("ExitGameButton", panelObject.transform, "退出游戏", new Color(0.11f, 0.1f, 0.12f, 1f), Muted);

        return new MainActions(panelObject, start, continueGame, abandon, settings, exit, status);
    }

    private static SettingsReferences CreateSettingsPanel(Transform parent)
    {
        GameObject overlay = CreateOverlay("SettingsPanel", parent);
        GameObject modal = CreateModal("SettingsWindow", overlay.transform, new Vector2(650f, 480f));

        CreateText(
            "SettingsTitleText",
            modal.transform,
            "设置",
            new Vector2(0.08f, 0.79f),
            new Vector2(0.92f, 0.94f),
            Vector2.zero,
            Vector2.zero,
            42f,
            TextAlignmentOptions.Left,
            Color.white,
            FontStyles.Bold);

        CreateText(
            "VolumeLabelText",
            modal.transform,
            "主音量",
            new Vector2(0.08f, 0.58f),
            new Vector2(0.45f, 0.7f),
            Vector2.zero,
            Vector2.zero,
            25f,
            TextAlignmentOptions.Left,
            Color.white,
            FontStyles.Normal);

        TMP_Text volumeValue = CreateText(
            "VolumeValueText",
            modal.transform,
            "100%",
            new Vector2(0.7f, 0.58f),
            new Vector2(0.92f, 0.7f),
            Vector2.zero,
            Vector2.zero,
            24f,
            TextAlignmentOptions.Right,
            Accent,
            FontStyles.Bold);

        Slider volume = CreateSlider("VolumeSlider", modal.transform, new Vector2(0.08f, 0.48f), new Vector2(0.92f, 0.57f));

        Toggle fullscreen = CreateToggle(
            "FullscreenToggle",
            modal.transform,
            "全屏显示",
            new Vector2(0.08f, 0.27f),
            new Vector2(0.92f, 0.41f));

        Button back = CreateAnchoredButton(
            "SettingsBackButton",
            modal.transform,
            "返回",
            new Vector2(0.61f, 0.07f),
            new Vector2(0.92f, 0.2f),
            Accent,
            Background);

        return new SettingsReferences(overlay, volume, volumeValue, fullscreen, back);
    }

    private static ConfirmationReferences CreateConfirmationPanel(Transform parent)
    {
        GameObject overlay = CreateOverlay("ConfirmationPanel", parent);
        GameObject modal = CreateModal("ConfirmationWindow", overlay.transform, new Vector2(600f, 330f));

        TMP_Text message = CreateText(
            "ConfirmationText",
            modal.transform,
            "确定执行此操作？",
            new Vector2(0.08f, 0.42f),
            new Vector2(0.92f, 0.87f),
            Vector2.zero,
            Vector2.zero,
            30f,
            TextAlignmentOptions.Center,
            Color.white,
            FontStyles.Bold);

        Button cancel = CreateAnchoredButton(
            "CancelButton",
            modal.transform,
            "取消",
            new Vector2(0.08f, 0.1f),
            new Vector2(0.45f, 0.32f),
            PanelLight,
            Color.white);
        Button confirm = CreateAnchoredButton(
            "ConfirmButton",
            modal.transform,
            "确定",
            new Vector2(0.55f, 0.1f),
            new Vector2(0.92f, 0.32f),
            Danger,
            Color.white);

        return new ConfirmationReferences(overlay, message, confirm, cancel);
    }

    private static Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("MainMenuCanvas", typeof(RectTransform), typeof(Canvas));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static UIScreen ConfigureScreen(GameObject root, UIRoute route, Selectable initialSelection)
    {
        UIScreen screen = root.AddComponent<UIScreen>();
        screen.Configure(route, root, initialSelection);
        return screen;
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Background;
        camera.orthographic = true;
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
    }

    private static void CreateEventSystem()
    {
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private static GameObject CreateOverlay(string name, Transform parent)
    {
        GameObject overlay = CreateUiObject(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image image = overlay.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.74f);
        return overlay;
    }

    private static GameObject CreateModal(string name, Transform parent, Vector2 size)
    {
        GameObject modal = CreateUiObject(
            name,
            parent,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            size);
        Image image = modal.AddComponent<Image>();
        image.color = Panel;
        Outline outline = modal.AddComponent<Outline>();
        outline.effectColor = new Color(0.25f, 0.34f, 0.42f, 0.85f);
        outline.effectDistance = new Vector2(2f, -2f);
        return modal;
    }

    private static Button CreateMenuButton(string name, Transform parent, string label, Color background, Color foreground)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(Button));
        buttonObject.layer = 5;
        buttonObject.transform.SetParent(parent, false);
        LayoutElement layout = buttonObject.GetComponent<LayoutElement>();
        layout.preferredHeight = 74f;

        return ConfigureButton(buttonObject, label, background, foreground);
    }

    private static Button CreateAnchoredButton(
        string name,
        Transform parent,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Color background,
        Color foreground)
    {
        GameObject buttonObject = CreateUiObject(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
        buttonObject.AddComponent<Image>();
        buttonObject.AddComponent<Button>();
        return ConfigureButton(buttonObject, label, background, foreground);
    }

    private static Button ConfigureButton(GameObject buttonObject, string label, Color background, Color foreground)
    {
        Image image = buttonObject.GetComponent<Image>();
        image.color = background;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.pressedColor = new Color(0.78f, 0.82f, 0.86f, 1f);
        colors.disabledColor = new Color(0.38f, 0.4f, 0.43f, 0.55f);
        colors.colorMultiplier = 1f;
        button.colors = colors;

        TMP_Text text = CreateText(
            "Label",
            buttonObject.transform,
            label,
            Vector2.zero,
            Vector2.one,
            new Vector2(24f, 0f),
            new Vector2(-24f, 0f),
            27f,
            TextAlignmentOptions.Center,
            foreground,
            FontStyles.Bold);
        text.raycastTarget = false;
        return button;
    }

    private static Slider CreateSlider(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject sliderObject = CreateUiObject(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
        Slider slider = sliderObject.AddComponent<Slider>();

        GameObject backgroundObject = CreateUiObject(
            "Background",
            sliderObject.transform,
            new Vector2(0f, 0.35f),
            new Vector2(1f, 0.65f),
            Vector2.zero,
            Vector2.zero);
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = PanelLight;

        GameObject fillArea = CreateUiObject(
            "Fill Area",
            sliderObject.transform,
            new Vector2(0f, 0.35f),
            new Vector2(1f, 0.65f),
            new Vector2(5f, 0f),
            new Vector2(-12f, 0f));
        GameObject fillObject = CreateUiObject("Fill", fillArea.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image fillImage = fillObject.AddComponent<Image>();
        fillImage.color = Accent;

        GameObject handleArea = CreateUiObject(
            "Handle Slide Area",
            sliderObject.transform,
            Vector2.zero,
            Vector2.one,
            new Vector2(10f, 0f),
            new Vector2(-10f, 0f));
        GameObject handleObject = CreateUiObject(
            "Handle",
            handleArea.transform,
            new Vector2(0f, 0.5f),
            new Vector2(0f, 0.5f),
            Vector2.zero,
            new Vector2(28f, 42f));
        Image handleImage = handleObject.AddComponent<Image>();
        handleImage.color = Color.white;

        slider.fillRect = fillObject.GetComponent<RectTransform>();
        slider.handleRect = handleObject.GetComponent<RectTransform>();
        slider.targetGraphic = handleImage;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        return slider;
    }

    private static Toggle CreateToggle(
        string name,
        Transform parent,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax)
    {
        GameObject toggleObject = CreateUiObject(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
        Toggle toggle = toggleObject.AddComponent<Toggle>();

        GameObject box = CreateUiObject(
            "Background",
            toggleObject.transform,
            new Vector2(0f, 0.5f),
            new Vector2(0f, 0.5f),
            new Vector2(24f, 0f),
            new Vector2(38f, 38f));
        Image boxImage = box.AddComponent<Image>();
        boxImage.color = PanelLight;

        GameObject check = CreateUiObject("Checkmark", box.transform, Vector2.zero, Vector2.one, new Vector2(7f, 7f), new Vector2(-7f, -7f));
        Image checkImage = check.AddComponent<Image>();
        checkImage.color = Accent;

        CreateText(
            "Label",
            toggleObject.transform,
            label,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            new Vector2(58f, 0f),
            Vector2.zero,
            25f,
            TextAlignmentOptions.Left,
            Color.white,
            FontStyles.Normal);

        toggle.targetGraphic = boxImage;
        toggle.graphic = checkImage;
        toggle.isOn = Screen.fullScreen;
        return toggle;
    }

    private static TMP_Text CreateLayoutText(
        string name,
        Transform parent,
        string value,
        float fontSize,
        Color color,
        float preferredHeight)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(LayoutElement), typeof(TextMeshProUGUI));
        textObject.layer = 5;
        textObject.transform.SetParent(parent, false);
        textObject.GetComponent<LayoutElement>().preferredHeight = preferredHeight;

        TMP_Text text = textObject.GetComponent<TextMeshProUGUI>();
        ApplyTextStyle(text, value, fontSize, TextAlignmentOptions.Center, color, FontStyles.Normal);
        return text;
    }

    private static void AddSpacer(Transform parent, float height)
    {
        GameObject spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
        spacer.layer = 5;
        spacer.transform.SetParent(parent, false);
        spacer.GetComponent<LayoutElement>().preferredHeight = height;
    }

    private static TMP_Text CreateText(
        string name,
        Transform parent,
        string value,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax,
        float fontSize,
        TextAlignmentOptions alignment,
        Color color,
        FontStyles style)
    {
        GameObject textObject = CreateUiObject(name, parent, anchorMin, anchorMax, offsetMin, offsetMax);
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        ApplyTextStyle(text, value, fontSize, alignment, color, style);
        return text;
    }

    private static void ApplyTextStyle(
        TMP_Text text,
        string value,
        float fontSize,
        TextAlignmentOptions alignment,
        Color color,
        FontStyles style)
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ChineseFontPath);
        if (font != null)
        {
            text.font = font;
        }

        text.text = value;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.fontStyle = style;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
    }

    private static Image CreateImage(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax,
        Color color)
    {
        GameObject imageObject = CreateUiObject(name, parent, anchorMin, anchorMax, offsetMin, offsetMax);
        Image image = imageObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static GameObject CreateUiObject(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax)
    {
        GameObject uiObject = new GameObject(name, typeof(RectTransform));
        uiObject.layer = 5;
        uiObject.transform.SetParent(parent, false);

        RectTransform rect = uiObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.localScale = Vector3.one;
        return uiObject;
    }

    private static void ConfigureBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true),
            new EditorBuildSettingsScene(GameplayScenePath, true)
        };
    }

    private readonly struct MainActions
    {
        public MainActions(
            GameObject panel,
            Button start,
            Button continueGame,
            Button abandon,
            Button settings,
            Button exit,
            TMP_Text sessionStatus)
        {
            this.panel = panel;
            this.start = start;
            this.continueGame = continueGame;
            this.abandon = abandon;
            this.settings = settings;
            this.exit = exit;
            this.sessionStatus = sessionStatus;
        }

        public readonly GameObject panel;
        public readonly Button start;
        public readonly Button continueGame;
        public readonly Button abandon;
        public readonly Button settings;
        public readonly Button exit;
        public readonly TMP_Text sessionStatus;
    }

    private readonly struct SettingsReferences
    {
        public SettingsReferences(GameObject panel, Slider volume, TMP_Text volumeValue, Toggle fullscreen, Button back)
        {
            this.panel = panel;
            this.volume = volume;
            this.volumeValue = volumeValue;
            this.fullscreen = fullscreen;
            this.back = back;
        }

        public readonly GameObject panel;
        public readonly Slider volume;
        public readonly TMP_Text volumeValue;
        public readonly Toggle fullscreen;
        public readonly Button back;
    }

    private readonly struct ConfirmationReferences
    {
        public ConfirmationReferences(GameObject panel, TMP_Text message, Button confirm, Button cancel)
        {
            this.panel = panel;
            this.message = message;
            this.confirm = confirm;
            this.cancel = cancel;
        }

        public readonly GameObject panel;
        public readonly TMP_Text message;
        public readonly Button confirm;
        public readonly Button cancel;
    }
}
