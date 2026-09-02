using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameplayPauseController : MonoBehaviour
{
    private const string FontResourcePath = "Fonts & Materials/SmileySans-Oblique SDF";
    private const string MainMenuSceneName = "MainMenu";

    private EnemySpawner spawner;
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private GameObject gameplayRouteRoot;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeValueText;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private UIRouter uiRouter;
    [SerializeField] private UIScreen gameplayScreen;
    [SerializeField] private UIScreen pauseScreen;
    [SerializeField] private UIScreen settingsScreen;
    private float previousTimeScale = 1f;

    public bool IsPaused { get; private set; }
    public bool HasSceneUiReferences => HasRequiredSceneObjects();

    private void Awake()
    {
        if (!HasRequiredSceneObjects())
        {
            Debug.LogError("Pause UI objects must be initialized in SampleScene.unity before entering Play Mode.", this);
            enabled = false;
            return;
        }

        BindActions();
        ConfigureNavigation();
        GameSessionState.ApplySettings();
        SyncSettingsUi();
        SetWindowVisible(false);
    }

    private void OnDisable()
    {
        ExitPauseState();
    }

    private void OnDestroy()
    {
        if (uiRouter != null)
        {
            uiRouter.RouteChanged -= HandleRouteChanged;
            uiRouter.SetBackInterceptor(null);
        }

        UnbindActions();
        ExitPauseState();
    }

    public static GameplayPauseController FindSceneController(EnemySpawner enemySpawner)
    {
        GameplayPauseController existing = FindObjectOfType<GameplayPauseController>(true);
        if (existing == null)
        {
            Debug.LogError("SampleScene is missing its GameplayPauseController scene object.");
            return null;
        }

        existing.spawner = enemySpawner;
        return existing;
    }

    public void PauseGame()
    {
        if (IsPaused || !CanPause())
        {
            return;
        }

        previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
        IsPaused = true;
        SetWindowVisible(true);
        Time.timeScale = 0f;

        if (uiRouter == null || !uiRouter.Navigate(UIRoute.Pause))
        {
            ShowMainPanel();
        }
    }

    public void ResumeGame()
    {
        if (!IsPaused)
        {
            return;
        }

        if (uiRouter != null && uiRouter.CurrentRoute != UIRoute.Gameplay)
        {
            if (uiRouter.CurrentRoute == UIRoute.Pause && uiRouter.Back())
            {
                return;
            }

            if (uiRouter.Initialize(UIRoute.Gameplay))
            {
                return;
            }
        }

        ExitPauseState();
    }

    private bool CanPause()
    {
        return spawner != null
            && spawner.IsLevelRunning
            && !spawner.HasRunEnded
            && Time.timeScale > 0f;
    }

    private void OpenSettings()
    {
        SyncSettingsUi();
        if (uiRouter != null && uiRouter.Navigate(UIRoute.Settings))
        {
            return;
        }

        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
        }
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    private void CloseSettings()
    {
        if (uiRouter != null && uiRouter.CurrentRoute == UIRoute.Settings && uiRouter.Back())
        {
            return;
        }

        ShowMainPanel();
    }

    private void ShowMainPanel()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void ReturnToMainMenu()
    {
        ExitPauseState();
        if (!Application.CanStreamedLevelBeLoaded(MainMenuSceneName))
        {
            Debug.LogError($"Main menu scene is not in Build Settings: {MainMenuSceneName}", this);
            return;
        }

        SceneManager.LoadScene(MainMenuSceneName);
    }

    private void SyncSettingsUi()
    {
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(GameSessionState.MasterVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(GameSessionState.Fullscreen);
        }

        UpdateVolumeValue(GameSessionState.MasterVolume);
    }

    private void HandleVolumeChanged(float value)
    {
        GameSessionState.SetMasterVolume(value);
        UpdateVolumeValue(value);
    }

    private void UpdateVolumeValue(float value)
    {
        if (volumeValueText != null)
        {
            volumeValueText.text = $"{Mathf.RoundToInt(Mathf.Clamp01(value) * 100f)}%";
        }
    }

    private static void HandleFullscreenChanged(bool fullscreen)
    {
        GameSessionState.SetFullscreen(fullscreen);
    }

    private void RestoreTimeScale()
    {
        if (!IsPaused)
        {
            return;
        }

        IsPaused = false;
        Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
    }

    private void ExitPauseState()
    {
        SetWindowVisible(false);
        RestoreTimeScale();
    }

    private void ConfigureNavigation()
    {
        gameplayScreen.Configure(UIRoute.Gameplay, gameplayRouteRoot, null);
        pauseScreen.Configure(UIRoute.Pause, mainPanel, resumeButton);
        settingsScreen.Configure(UIRoute.Settings, settingsPanel, volumeSlider);

        uiRouter.Register(gameplayScreen);
        uiRouter.Register(pauseScreen);
        uiRouter.Register(settingsScreen);
        uiRouter.SetBackInterceptor(HandleBackRequest);
        uiRouter.RouteChanged += HandleRouteChanged;
        uiRouter.Initialize(UIRoute.Gameplay);
    }

#if UNITY_EDITOR
    public void BuildSceneUi()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Pause UI scene objects cannot be created during Play Mode.", this);
            return;
        }

        if (windowRoot == null)
        {
            BuildUi();
        }

        if (gameplayRouteRoot == null)
        {
            gameplayRouteRoot = new GameObject("GameplayRoute", typeof(RectTransform));
            gameplayRouteRoot.transform.SetParent(transform, false);
        }

        uiRouter = uiRouter != null ? uiRouter : GetComponent<UIRouter>();
        if (uiRouter == null)
        {
            uiRouter = gameObject.AddComponent<UIRouter>();
        }

        gameplayScreen = gameplayScreen != null
            ? gameplayScreen
            : GetOrAddScreen(gameplayRouteRoot);
        pauseScreen = pauseScreen != null ? pauseScreen : GetOrAddScreen(mainPanel);
        settingsScreen = settingsScreen != null ? settingsScreen : GetOrAddScreen(settingsPanel);
        gameplayScreen.Configure(UIRoute.Gameplay, gameplayRouteRoot, null);
        pauseScreen.Configure(UIRoute.Pause, mainPanel, resumeButton);
        settingsScreen.Configure(UIRoute.Settings, settingsPanel, volumeSlider);
        SetWindowVisible(false);
    }
#endif

    private bool HasRequiredSceneObjects()
    {
        return windowRoot != null
            && gameplayRouteRoot != null
            && mainPanel != null
            && settingsPanel != null
            && resumeButton != null
            && settingsButton != null
            && mainMenuButton != null
            && settingsBackButton != null
            && volumeSlider != null
            && volumeValueText != null
            && fullscreenToggle != null
            && uiRouter != null
            && gameplayScreen != null
            && pauseScreen != null
            && settingsScreen != null;
    }

#if UNITY_EDITOR
    private static UIScreen GetOrAddScreen(GameObject root)
    {
        UIScreen screen = root.GetComponent<UIScreen>();
        return screen != null ? screen : root.AddComponent<UIScreen>();
    }
#endif

    private void BindActions()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        settingsBackButton.onClick.AddListener(CloseSettings);
        volumeSlider.onValueChanged.AddListener(HandleVolumeChanged);
        fullscreenToggle.onValueChanged.AddListener(HandleFullscreenChanged);
    }

    private void UnbindActions()
    {
        resumeButton?.onClick.RemoveListener(ResumeGame);
        settingsButton?.onClick.RemoveListener(OpenSettings);
        mainMenuButton?.onClick.RemoveListener(ReturnToMainMenu);
        settingsBackButton?.onClick.RemoveListener(CloseSettings);
        volumeSlider?.onValueChanged.RemoveListener(HandleVolumeChanged);
        fullscreenToggle?.onValueChanged.RemoveListener(HandleFullscreenChanged);
    }

    private bool HandleBackRequest()
    {
        if (IsPaused || !CanPause())
        {
            return false;
        }

        PauseGame();
        return true;
    }

    private void HandleRouteChanged(UIRoute route)
    {
        if (route == UIRoute.Gameplay)
        {
            ExitPauseState();
        }
    }

    private void SetWindowVisible(bool visible)
    {
        if (windowRoot != null && windowRoot.activeSelf != visible)
        {
            windowRoot.SetActive(visible);
        }
    }

#if UNITY_EDITOR
    private void BuildUi()
    {
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>(FontResourcePath);
        GameObject canvasObject = new GameObject(
            "PauseCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        canvasObject.layer = 5;
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 250;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        windowRoot = CreateUiObject("PauseWindow", canvasObject.transform);
        Stretch(windowRoot.GetComponent<RectTransform>());
        Image dimmer = windowRoot.AddComponent<Image>();
        dimmer.color = new Color(0.01f, 0.012f, 0.018f, 0.82f);

        mainPanel = CreatePanel("PauseMainPanel", windowRoot.transform, new Vector2(620f, 650f));
        TMP_Text title = CreateText("Title", mainPanel.transform, font, 54f, FontStyles.Bold);
        SetRect(title.rectTransform, new Vector2(0.08f, 0.79f), new Vector2(0.92f, 0.94f));
        title.text = "游戏暂停";
        title.alignment = TextAlignmentOptions.Center;

        resumeButton = CreateButton(
            "ResumeButton",
            mainPanel.transform,
            font,
            "继续游戏",
            new Vector2(0.13f, 0.56f),
            new Vector2(0.87f, 0.69f),
            new Color(0.22f, 0.58f, 0.3f, 1f));
        settingsButton = CreateButton(
            "SettingsButton",
            mainPanel.transform,
            font,
            "设置",
            new Vector2(0.13f, 0.37f),
            new Vector2(0.87f, 0.5f),
            new Color(0.23f, 0.27f, 0.35f, 1f));
        mainMenuButton = CreateButton(
            "MainMenuButton",
            mainPanel.transform,
            font,
            "返回主菜单",
            new Vector2(0.13f, 0.18f),
            new Vector2(0.87f, 0.31f),
            new Color(0.32f, 0.25f, 0.25f, 1f));

        TMP_Text hint = CreateText("Hint", mainPanel.transform, font, 19f, FontStyles.Normal);
        SetRect(hint.rectTransform, new Vector2(0.08f, 0.04f), new Vector2(0.92f, 0.12f));
        hint.text = "按 ESC 继续游戏";
        hint.color = new Color(0.68f, 0.7f, 0.76f, 1f);
        hint.alignment = TextAlignmentOptions.Center;

        BuildSettingsPanel(windowRoot.transform, font);
    }

    private void BuildSettingsPanel(Transform parent, TMP_FontAsset font)
    {
        settingsPanel = CreatePanel("PauseSettingsPanel", parent, new Vector2(680f, 650f));

        TMP_Text title = CreateText("Title", settingsPanel.transform, font, 48f, FontStyles.Bold);
        SetRect(title.rectTransform, new Vector2(0.08f, 0.81f), new Vector2(0.92f, 0.94f));
        title.text = "设置";
        title.alignment = TextAlignmentOptions.Center;

        TMP_Text volumeLabel = CreateText("VolumeLabel", settingsPanel.transform, font, 26f, FontStyles.Bold);
        SetRect(volumeLabel.rectTransform, new Vector2(0.12f, 0.63f), new Vector2(0.42f, 0.72f));
        volumeLabel.text = "主音量";
        volumeLabel.alignment = TextAlignmentOptions.MidlineLeft;

        volumeSlider = CreateSlider("VolumeSlider", settingsPanel.transform);
        SetRect(volumeSlider.GetComponent<RectTransform>(), new Vector2(0.39f, 0.64f), new Vector2(0.76f, 0.71f));

        volumeValueText = CreateText("VolumeValue", settingsPanel.transform, font, 24f, FontStyles.Bold);
        SetRect(volumeValueText.rectTransform, new Vector2(0.77f, 0.63f), new Vector2(0.89f, 0.72f));
        volumeValueText.alignment = TextAlignmentOptions.Center;

        fullscreenToggle = CreateToggle("FullscreenToggle", settingsPanel.transform, font, "全屏");
        SetRect(fullscreenToggle.GetComponent<RectTransform>(), new Vector2(0.18f, 0.43f), new Vector2(0.82f, 0.55f));

        settingsBackButton = CreateButton(
            "SettingsBackButton",
            settingsPanel.transform,
            font,
            "返回",
            new Vector2(0.18f, 0.16f),
            new Vector2(0.82f, 0.29f),
            new Color(0.23f, 0.27f, 0.35f, 1f));

        settingsPanel.SetActive(false);
    }

    private static GameObject CreatePanel(string objectName, Transform parent, Vector2 size)
    {
        GameObject panel = CreateUiObject(objectName, parent);
        Center(panel.GetComponent<RectTransform>(), size);
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.065f, 0.07f, 0.085f, 0.98f);
        return panel;
    }

    private static Button CreateButton(
        string objectName,
        Transform parent,
        TMP_FontAsset font,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Color color)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        SetRect(buttonObject.GetComponent<RectTransform>(), anchorMin, anchorMax);
        Image image = buttonObject.AddComponent<Image>();
        image.color = color;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        TMP_Text text = CreateText("Text", buttonObject.transform, font, 28f, FontStyles.Bold);
        Stretch(text.rectTransform);
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        return button;
    }

    private static Slider CreateSlider(string objectName, Transform parent)
    {
        GameObject sliderObject = CreateUiObject(objectName, parent);
        Image background = sliderObject.AddComponent<Image>();
        background.color = new Color(0.13f, 0.14f, 0.18f, 1f);

        GameObject fillObject = CreateUiObject("Fill", sliderObject.transform);
        SetRect(fillObject.GetComponent<RectTransform>(), new Vector2(0f, 0.15f), new Vector2(1f, 0.85f));
        Image fill = fillObject.AddComponent<Image>();
        fill.color = new Color(0.3f, 0.72f, 0.4f, 1f);

        GameObject handleObject = CreateUiObject("Handle", sliderObject.transform);
        RectTransform handleRect = handleObject.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0f, 0.5f);
        handleRect.anchorMax = new Vector2(0f, 0.5f);
        handleRect.sizeDelta = new Vector2(30f, 46f);
        Image handle = handleObject.AddComponent<Image>();
        handle.color = Color.white;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.fillRect = fillObject.GetComponent<RectTransform>();
        slider.handleRect = handleRect;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private static Toggle CreateToggle(string objectName, Transform parent, TMP_FontAsset font, string label)
    {
        GameObject toggleObject = CreateUiObject(objectName, parent);
        Toggle toggle = toggleObject.AddComponent<Toggle>();

        GameObject backgroundObject = CreateUiObject("Background", toggleObject.transform);
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 0.5f);
        backgroundRect.anchorMax = new Vector2(0f, 0.5f);
        backgroundRect.pivot = new Vector2(0f, 0.5f);
        backgroundRect.anchoredPosition = new Vector2(12f, 0f);
        backgroundRect.sizeDelta = new Vector2(56f, 56f);
        Image background = backgroundObject.AddComponent<Image>();
        background.color = new Color(0.14f, 0.15f, 0.2f, 1f);

        GameObject checkmarkObject = CreateUiObject("Checkmark", backgroundObject.transform);
        SetRect(checkmarkObject.GetComponent<RectTransform>(), new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f));
        Image checkmark = checkmarkObject.AddComponent<Image>();
        checkmark.color = new Color(0.3f, 0.78f, 0.42f, 1f);

        TMP_Text text = CreateText("Label", toggleObject.transform, font, 27f, FontStyles.Bold);
        SetRect(text.rectTransform, new Vector2(0.18f, 0f), Vector2.one);
        text.text = label;
        text.alignment = TextAlignmentOptions.MidlineLeft;

        toggle.targetGraphic = background;
        toggle.graphic = checkmark;
        return toggle;
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
#endif
}
