using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuController : MonoBehaviour
{
    private enum ConfirmationAction
    {
        None,
        AbandonRun,
        ExitGame
    }

    [Header("Scene")]
    [SerializeField] private string gameplaySceneName = "SampleScene";

    [Header("Main Actions")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button abandonButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text sessionStatusText;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeValueText;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button settingsBackButton;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TMP_Text confirmationText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private ConfirmationAction pendingConfirmation;

    private void Awake()
    {
        AutoBindReferences();
        BindActions();
        GameSessionState.ApplySettings();
        SyncSettingsUi();
        CloseOverlays();
        RefreshSessionState();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (confirmationPanel != null && confirmationPanel.activeSelf)
        {
            CancelConfirmation();
        }
        else if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
        }
    }

    private void OnDestroy()
    {
        UnbindActions();
    }

    public void Configure(
        Button newStartButton,
        Button newContinueButton,
        Button newAbandonButton,
        Button newSettingsButton,
        Button newExitButton,
        TMP_Text newSessionStatusText,
        GameObject newSettingsPanel,
        Slider newVolumeSlider,
        TMP_Text newVolumeValueText,
        Toggle newFullscreenToggle,
        Button newSettingsBackButton,
        GameObject newConfirmationPanel,
        TMP_Text newConfirmationText,
        Button newConfirmButton,
        Button newCancelButton)
    {
        startButton = newStartButton;
        continueButton = newContinueButton;
        abandonButton = newAbandonButton;
        settingsButton = newSettingsButton;
        exitButton = newExitButton;
        sessionStatusText = newSessionStatusText;
        settingsPanel = newSettingsPanel;
        volumeSlider = newVolumeSlider;
        volumeValueText = newVolumeValueText;
        fullscreenToggle = newFullscreenToggle;
        settingsBackButton = newSettingsBackButton;
        confirmationPanel = newConfirmationPanel;
        confirmationText = newConfirmationText;
        confirmButton = newConfirmButton;
        cancelButton = newCancelButton;
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        startButton = startButton != null ? startButton : FindComponent<Button>("StartGameButton");
        continueButton = continueButton != null ? continueButton : FindComponent<Button>("ContinueGameButton");
        abandonButton = abandonButton != null ? abandonButton : FindComponent<Button>("AbandonRunButton");
        settingsButton = settingsButton != null ? settingsButton : FindComponent<Button>("SettingsButton");
        exitButton = exitButton != null ? exitButton : FindComponent<Button>("ExitGameButton");
        sessionStatusText = sessionStatusText != null ? sessionStatusText : FindComponent<TMP_Text>("SessionStatusText");

        settingsPanel = settingsPanel != null ? settingsPanel : FindObject("SettingsPanel");
        volumeSlider = volumeSlider != null ? volumeSlider : FindComponent<Slider>("VolumeSlider");
        volumeValueText = volumeValueText != null ? volumeValueText : FindComponent<TMP_Text>("VolumeValueText");
        fullscreenToggle = fullscreenToggle != null ? fullscreenToggle : FindComponent<Toggle>("FullscreenToggle");
        settingsBackButton = settingsBackButton != null ? settingsBackButton : FindComponent<Button>("SettingsBackButton");

        confirmationPanel = confirmationPanel != null ? confirmationPanel : FindObject("ConfirmationPanel");
        confirmationText = confirmationText != null ? confirmationText : FindComponent<TMP_Text>("ConfirmationText");
        confirmButton = confirmButton != null ? confirmButton : FindComponent<Button>("ConfirmButton");
        cancelButton = cancelButton != null ? cancelButton : FindComponent<Button>("CancelButton");
    }

    public void StartNewGame()
    {
        GameSessionState.BeginNewRun();
        LoadGameplayScene();
    }

    public void ContinueGame()
    {
        if (!GameSessionState.HasActiveRun)
        {
            RefreshSessionState();
            return;
        }

        LoadGameplayScene();
    }

    public void RequestAbandonRun()
    {
        if (!GameSessionState.HasActiveRun)
        {
            RefreshSessionState();
            return;
        }

        OpenConfirmation(
            ConfirmationAction.AbandonRun,
            "放弃当前游戏？\n当前进行中的游戏标记将被清除。",
            "放弃游戏");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void RequestExitGame()
    {
        OpenConfirmation(ConfirmationAction.ExitGame, "确定退出游戏？", "退出游戏");
    }

    public void ConfirmPendingAction()
    {
        ConfirmationAction action = pendingConfirmation;
        CancelConfirmation();

        switch (action)
        {
            case ConfirmationAction.AbandonRun:
                GameSessionState.AbandonRun();
                RefreshSessionState();
                break;
            case ConfirmationAction.ExitGame:
                QuitGame();
                break;
        }
    }

    public void CancelConfirmation()
    {
        pendingConfirmation = ConfirmationAction.None;
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    private void OpenConfirmation(ConfirmationAction action, string message, string confirmLabel)
    {
        pendingConfirmation = action;
        if (confirmationText != null)
        {
            confirmationText.text = message;
        }

        TMP_Text confirmLabelText = confirmButton != null
            ? confirmButton.GetComponentInChildren<TMP_Text>(true)
            : null;
        if (confirmLabelText != null)
        {
            confirmLabelText.text = confirmLabel;
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
    }

    private void LoadGameplayScene()
    {
        if (!Application.CanStreamedLevelBeLoaded(gameplaySceneName))
        {
            if (sessionStatusText != null)
            {
                sessionStatusText.text = $"无法加载场景：{gameplaySceneName}";
            }

            Debug.LogError($"Gameplay scene is not in Build Settings: {gameplaySceneName}", this);
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }

    private void RefreshSessionState()
    {
        bool hasActiveRun = GameSessionState.HasActiveRun;
        if (continueButton != null)
        {
            continueButton.interactable = hasActiveRun;
        }

        if (abandonButton != null)
        {
            abandonButton.interactable = hasActiveRun;
        }

        if (sessionStatusText != null)
        {
            sessionStatusText.text = hasActiveRun
                ? "检测到进行中的游戏"
                : "当前没有进行中的游戏";
        }
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

    private void CloseOverlays()
    {
        CloseSettings();
        CancelConfirmation();
    }

    private void BindActions()
    {
        AddListener(startButton, StartNewGame);
        AddListener(continueButton, ContinueGame);
        AddListener(abandonButton, RequestAbandonRun);
        AddListener(settingsButton, OpenSettings);
        AddListener(exitButton, RequestExitGame);
        AddListener(settingsBackButton, CloseSettings);
        AddListener(confirmButton, ConfirmPendingAction);
        AddListener(cancelButton, CancelConfirmation);
        volumeSlider?.onValueChanged.AddListener(HandleVolumeChanged);
        fullscreenToggle?.onValueChanged.AddListener(HandleFullscreenChanged);
    }

    private void UnbindActions()
    {
        RemoveListener(startButton, StartNewGame);
        RemoveListener(continueButton, ContinueGame);
        RemoveListener(abandonButton, RequestAbandonRun);
        RemoveListener(settingsButton, OpenSettings);
        RemoveListener(exitButton, RequestExitGame);
        RemoveListener(settingsBackButton, CloseSettings);
        RemoveListener(confirmButton, ConfirmPendingAction);
        RemoveListener(cancelButton, CancelConfirmation);
        volumeSlider?.onValueChanged.RemoveListener(HandleVolumeChanged);
        fullscreenToggle?.onValueChanged.RemoveListener(HandleFullscreenChanged);
    }

    private static void AddListener(Button button, UnityEngine.Events.UnityAction action)
    {
        button?.onClick.AddListener(action);
    }

    private static void RemoveListener(Button button, UnityEngine.Events.UnityAction action)
    {
        button?.onClick.RemoveListener(action);
    }

    private GameObject FindObject(params string[] names)
    {
        Transform child = FindTransform(names);
        return child != null ? child.gameObject : null;
    }

    private T FindComponent<T>(params string[] names) where T : Component
    {
        Transform child = FindTransform(names);
        return child != null ? child.GetComponent<T>() : null;
    }

    private Transform FindTransform(params string[] names)
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

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
