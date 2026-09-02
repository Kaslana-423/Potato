using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameplayPauseController : MonoBehaviour
{
    private const string FontResourcePath = "Fonts & Materials/SmileySans-Oblique SDF";
    private const string MainMenuSceneName = "MainMenu";

    private EnemySpawner spawner;
    private GameObject windowRoot;
    private GameObject mainPanel;
    private GameObject settingsPanel;
    private Slider volumeSlider;
    private TMP_Text volumeValueText;
    private Toggle fullscreenToggle;
    private float previousTimeScale = 1f;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        BuildUi();
        GameSessionState.ApplySettings();
        SyncSettingsUi();
        SetWindowVisible(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (IsPaused)
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                ResumeGame();
            }

            return;
        }

        if (CanPause())
        {
            PauseGame();
        }
    }

    private void OnDisable()
    {
        RestoreTimeScale();
    }

    private void OnDestroy()
    {
        RestoreTimeScale();
    }

    public static GameplayPauseController GetOrCreate(EnemySpawner enemySpawner)
    {
        GameplayPauseController existing = FindObjectOfType<GameplayPauseController>(true);
        if (existing == null)
        {
            GameObject controllerObject = new GameObject("GameplayPauseController");
            existing = controllerObject.AddComponent<GameplayPauseController>();
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
        ShowMainPanel();
        SetWindowVisible(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!IsPaused)
        {
            return;
        }

        SetWindowVisible(false);
        RestoreTimeScale();
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
        ResumeGame();
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

    private void SetWindowVisible(bool visible)
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

        CreateButton(
            "ResumeButton",
            mainPanel.transform,
            font,
            "继续游戏",
            new Vector2(0.13f, 0.56f),
            new Vector2(0.87f, 0.69f),
            new Color(0.22f, 0.58f, 0.3f, 1f),
            ResumeGame);
        CreateButton(
            "SettingsButton",
            mainPanel.transform,
            font,
            "设置",
            new Vector2(0.13f, 0.37f),
            new Vector2(0.87f, 0.5f),
            new Color(0.23f, 0.27f, 0.35f, 1f),
            OpenSettings);
        CreateButton(
            "MainMenuButton",
            mainPanel.transform,
            font,
            "返回主菜单",
            new Vector2(0.13f, 0.18f),
            new Vector2(0.87f, 0.31f),
            new Color(0.32f, 0.25f, 0.25f, 1f),
            ReturnToMainMenu);

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
        volumeSlider.onValueChanged.AddListener(HandleVolumeChanged);

        volumeValueText = CreateText("VolumeValue", settingsPanel.transform, font, 24f, FontStyles.Bold);
        SetRect(volumeValueText.rectTransform, new Vector2(0.77f, 0.63f), new Vector2(0.89f, 0.72f));
        volumeValueText.alignment = TextAlignmentOptions.Center;

        fullscreenToggle = CreateToggle("FullscreenToggle", settingsPanel.transform, font, "全屏");
        SetRect(fullscreenToggle.GetComponent<RectTransform>(), new Vector2(0.18f, 0.43f), new Vector2(0.82f, 0.55f));
        fullscreenToggle.onValueChanged.AddListener(HandleFullscreenChanged);

        CreateButton(
            "SettingsBackButton",
            settingsPanel.transform,
            font,
            "返回",
            new Vector2(0.18f, 0.16f),
            new Vector2(0.82f, 0.29f),
            new Color(0.23f, 0.27f, 0.35f, 1f),
            CloseSettings);

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
        Color color,
        UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        SetRect(buttonObject.GetComponent<RectTransform>(), anchorMin, anchorMax);
        Image image = buttonObject.AddComponent<Image>();
        image.color = color;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

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
}
