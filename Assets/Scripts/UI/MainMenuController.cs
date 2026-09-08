using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private GameObject mainActionsPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private RectTransform selectedTag;
    [SerializeField] private float selectedTagVerticalAdjustment = 1f;

    [Header("Main Action Tag Offsets")]
    [SerializeField] private Vector2 startTagOffset;
    [SerializeField] private Vector2 continueTagOffset;
    [SerializeField] private Vector2 settingsTagOffset;
    [SerializeField] private Vector2 statsTagOffset;
    [SerializeField] private Vector2 exitTagOffset;

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

    [Header("Navigation")]
    [SerializeField] private MainMenuFlowView navigationView;
    [SerializeField] private UIRouter uiRouter;
    [SerializeField] private UIScreen titleScreen;
    [SerializeField] private UIScreen saveSelectScreen;
    [SerializeField] private UIScreen mainMenuScreen;
    [SerializeField] private UIScreen characterSelectScreen;
    [SerializeField] private UIScreen settingsScreen;

    private ConfirmationAction pendingConfirmation;
    private GameObject selectionBeforeConfirmation;
    private int titleInputStartFrame;
    private int selectedSaveSlotIndex;
    private int selectedCharacterIndex;
    private bool deleteSaveFocused;
    private bool deleteMode;
    private bool hasLastMousePosition;
    private Vector3 lastMousePosition;
    private Button[] mainActionButtons;
    private int selectedMainActionIndex;
    private bool hasLastMainActionMousePosition;
    private Vector3 lastMainActionMousePosition;
    private Vector3 selectedTagOffset;
    private Vector3 selectedTagInitialLocalPosition;
    private bool hasSelectedTagOffset;
    private readonly List<RaycastResult> mainActionRaycastResults = new List<RaycastResult>();
    private Button abandonButton;
    private TMP_Text sessionStatusText;
    private TMP_Text continueButtonLabel;
    private Color continueButtonLabelColor;
    private bool hasContinueButtonLabelColor;

    public bool HasSceneNavigationReferences => mainActionsPanel != null
        && navigationView != null
        && uiRouter != null
        && titleScreen != null
        && saveSelectScreen != null
        && mainMenuScreen != null
        && characterSelectScreen != null
        && settingsScreen != null;

    private void Awake()
    {
        AutoBindReferences();
        ConfigureNavigation();
        BindActions();
        GameSessionState.ApplySettings();
        SyncSettingsUi();
        RefreshSaveSlots();
        RefreshSessionState();
        CancelConfirmation();

        if (uiRouter != null)
        {
            InitializeNavigation();
        }
        else
        {
            CloseOverlays();
        }
    }

    private void OnDestroy()
    {
        if (uiRouter != null)
        {
            uiRouter.RouteChanged -= HandleRouteChanged;
            uiRouter.SetBackInterceptor(null);
        }

        UnbindActions();
    }

    private void LateUpdate()
    {
        if (pendingConfirmation != ConfirmationAction.None)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ConfirmPendingAction();
            }

            return;
        }

        if (uiRouter == null)
        {
            return;
        }

        if (uiRouter.CurrentRoute == UIRoute.Title)
        {
            if (Time.frameCount < titleInputStartFrame)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
                return;
            }

            if (Input.anyKeyDown
                    || Input.GetMouseButtonDown(0)
                    || Input.GetMouseButtonDown(1)
                    || Input.GetMouseButtonDown(2))
            {
                OpenSaveSelect();
            }

            return;
        }

        if (uiRouter.CurrentRoute == UIRoute.SaveSelect)
        {
            HandleSaveSelectInput();
        }
        else if (uiRouter.CurrentRoute == UIRoute.MainMenu)
        {
            HandleMainActionInput();
        }
        else if (uiRouter.CurrentRoute == UIRoute.CharacterSelect)
        {
            HandleCharacterSelectInput();
        }
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
        mainActionsPanel = mainActionsPanel != null ? mainActionsPanel : FindObject("MainActionsPanel");
        startButton = startButton != null ? startButton : FindComponent<Button>("StartGameButton");
        continueButton = continueButton != null ? continueButton : FindComponent<Button>("ContinueGameButton");
        abandonButton = abandonButton != null ? abandonButton : FindComponent<Button>("AbandonRunButton");
        statsButton = statsButton != null ? statsButton : FindComponent<Button>("StatsButton");
        settingsButton = settingsButton != null ? settingsButton : FindComponent<Button>("SettingsButton");
        exitButton = exitButton != null ? exitButton : FindComponent<Button>("ExitGameButton", "ExitButton");
        selectedTag = selectedTag != null ? selectedTag : FindComponent<RectTransform>("SelectedTag");
        sessionStatusText = sessionStatusText != null ? sessionStatusText : FindComponent<TMP_Text>("SessionStatusText");

        BuildMainActionButtons();
        CaptureSelectedTagOffset();
        CaptureContinueButtonLabelColor();

        settingsPanel = settingsPanel != null ? settingsPanel : FindObject("SettingsPanel");
        volumeSlider = volumeSlider != null ? volumeSlider : FindComponent<Slider>("VolumeSlider");
        volumeValueText = volumeValueText != null ? volumeValueText : FindComponent<TMP_Text>("VolumeValueText");
        fullscreenToggle = fullscreenToggle != null ? fullscreenToggle : FindComponent<Toggle>("FullscreenToggle");
        settingsBackButton = settingsBackButton != null ? settingsBackButton : FindComponent<Button>("SettingsBackButton");

        confirmationPanel = confirmationPanel != null ? confirmationPanel : FindObject("ConfirmationPanel");
        confirmationText = confirmationText != null ? confirmationText : FindComponent<TMP_Text>("ConfirmationText");
        confirmButton = confirmButton != null ? confirmButton : FindComponent<Button>("ConfirmButton");
        cancelButton = cancelButton != null ? cancelButton : FindComponent<Button>("CancelButton");
        navigationView = navigationView != null ? navigationView : GetComponent<MainMenuFlowView>();
        uiRouter = uiRouter != null ? uiRouter : GetComponent<UIRouter>();
        navigationView?.AutoBindSceneVisuals();
    }

    public void StartNewGame()
    {
        if (GameSessionState.HasActiveRun)
        {
            GameSessionState.AbandonRun();
            RefreshSessionState();
        }

        OpenCharacterSelect();
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
        if (uiRouter != null && uiRouter.Navigate(UIRoute.Settings))
        {
            return;
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OpenSaveSelect()
    {
        uiRouter?.Navigate(UIRoute.SaveSelect);
    }

    public void CloseSaveSelect()
    {
        if (uiRouter != null && uiRouter.CurrentRoute == UIRoute.SaveSelect)
        {
            uiRouter.Back();
        }
    }

    public void OpenCharacterSelect()
    {
        uiRouter?.Navigate(UIRoute.CharacterSelect);
    }

    public void CloseCharacterSelect()
    {
        if (uiRouter != null && uiRouter.CurrentRoute == UIRoute.CharacterSelect)
        {
            uiRouter.Back();
        }
    }

    public void StartSelectedCharacter()
    {
        CharacterDefinition selectedCharacter = navigationView?.GetCharacter(selectedCharacterIndex);
        if (selectedCharacter == null || !selectedCharacter.Unlocked)
        {
            return;
        }

        GameSessionState.BeginNewRun(selectedCharacter.Id);
        LoadGameplayScene();
    }

    public void CloseSettings()
    {
        if (uiRouter != null && uiRouter.CurrentRoute == UIRoute.Settings && uiRouter.Back())
        {
            return;
        }

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
        bool wasOpen = confirmationPanel != null && confirmationPanel.activeSelf;
        pendingConfirmation = ConfirmationAction.None;
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        if (wasOpen && selectionBeforeConfirmation != null && selectionBeforeConfirmation.activeInHierarchy)
        {
            EventSystem.current?.SetSelectedGameObject(selectionBeforeConfirmation);
        }

        selectionBeforeConfirmation = null;
    }

    private void OpenConfirmation(ConfirmationAction action, string message, string confirmLabel)
    {
        selectionBeforeConfirmation = EventSystem.current != null
            ? EventSystem.current.currentSelectedGameObject
            : null;
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
            cancelButton?.Select();
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
            continueButton.gameObject.SetActive(true);
            continueButton.interactable = hasActiveRun;
        }

        UpdateContinueButtonVisual(hasActiveRun);

        if (abandonButton != null)
        {
            abandonButton.gameObject.SetActive(hasActiveRun);
            abandonButton.interactable = hasActiveRun;
        }

        if (mainMenuScreen != null && mainActionsPanel != null)
        {
            mainMenuScreen.Configure(
                UIRoute.MainMenu,
                mainActionsPanel,
                hasActiveRun ? continueButton : startButton);
        }

        if (sessionStatusText != null)
        {
            sessionStatusText.text = hasActiveRun
                ? "检测到进行中的游戏"
                : "当前没有进行中的游戏";
        }

        if (uiRouter != null && uiRouter.CurrentRoute == UIRoute.MainMenu)
        {
            ApplyMainActionSelection();
        }
    }

    private void CaptureContinueButtonLabelColor()
    {
        continueButtonLabel = continueButton != null
            ? continueButton.GetComponentInChildren<TMP_Text>(true)
            : null;
        if (continueButtonLabel == null)
        {
            return;
        }

        continueButtonLabelColor = continueButtonLabel.color;
        hasContinueButtonLabelColor = true;
    }

    private void UpdateContinueButtonVisual(bool available)
    {
        if (!hasContinueButtonLabelColor)
        {
            CaptureContinueButtonLabelColor();
        }

        if (continueButtonLabel == null)
        {
            return;
        }

        continueButtonLabel.color = available
            ? continueButtonLabelColor
            : new Color(
                continueButtonLabelColor.r * 0.35f,
                continueButtonLabelColor.g * 0.35f,
                continueButtonLabelColor.b * 0.35f,
                continueButtonLabelColor.a);
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

    public void ConfigureNavigationReferences(
        GameObject newMainActionsPanel,
        MainMenuFlowView newNavigationView,
        UIRouter newUiRouter,
        UIScreen newTitleScreen,
        UIScreen newSaveSelectScreen,
        UIScreen newMainMenuScreen,
        UIScreen newCharacterSelectScreen,
        UIScreen newSettingsScreen)
    {
        mainActionsPanel = newMainActionsPanel;
        navigationView = newNavigationView;
        uiRouter = newUiRouter;
        titleScreen = newTitleScreen;
        saveSelectScreen = newSaveSelectScreen;
        mainMenuScreen = newMainMenuScreen;
        characterSelectScreen = newCharacterSelectScreen;
        settingsScreen = newSettingsScreen;
    }

    private void ConfigureNavigation()
    {
        if (mainActionsPanel == null
            || settingsPanel == null
            || navigationView == null
            || navigationView.TitlePanel == null
            || navigationView.SaveSelectPanel == null
            || navigationView.CharacterSelectPanel == null)
        {
            Debug.LogWarning("Main menu navigation is missing one or more required pages.", this);
            return;
        }

        if (uiRouter == null
            || titleScreen == null
            || saveSelectScreen == null
            || mainMenuScreen == null
            || characterSelectScreen == null
            || settingsScreen == null)
        {
            Debug.LogError("Main menu navigation components must be initialized in MainMenu.unity.", this);
            return;
        }

        titleScreen.Configure(UIRoute.Title, navigationView.TitlePanel, navigationView.TitleContinueButton);
        saveSelectScreen.Configure(UIRoute.SaveSelect, navigationView.SaveSelectPanel, navigationView.FirstSaveSlotButton);
        mainMenuScreen.Configure(UIRoute.MainMenu, mainActionsPanel, startButton);
        characterSelectScreen.Configure(
            UIRoute.CharacterSelect,
            navigationView.CharacterSelectPanel,
            navigationView.CharacterStartButton);
        settingsScreen.Configure(UIRoute.Settings, settingsPanel, volumeSlider);
        uiRouter.Register(titleScreen);
        uiRouter.Register(saveSelectScreen);
        uiRouter.Register(mainMenuScreen);
        uiRouter.Register(characterSelectScreen);
        uiRouter.Register(settingsScreen);
        uiRouter.SetBackInterceptor(HandleBackRequest);
        uiRouter.RouteChanged -= HandleRouteChanged;
        uiRouter.RouteChanged += HandleRouteChanged;
    }

    private void InitializeNavigation()
    {
        uiRouter.Initialize(UIRoute.Title);
    }

    private void HandleRouteChanged(UIRoute route)
    {
        navigationView?.ApplyRouteVisuals(route);

        if (route == UIRoute.Title)
        {
            titleInputStartFrame = Time.frameCount + 1;
        }
        else if (route == UIRoute.SaveSelect)
        {
            InitializeSaveSelect();
        }
        else if (route == UIRoute.MainMenu)
        {
            RefreshSessionState();
            InitializeMainActionSelection();
        }
        else if (route == UIRoute.CharacterSelect)
        {
            InitializeCharacterSelection();
        }
    }

    private bool HandleBackRequest()
    {
        if (uiRouter != null && uiRouter.CurrentRoute == UIRoute.SaveSelect && deleteMode)
        {
            ExitDeleteMode();
            return true;
        }

        if (confirmationPanel == null || !confirmationPanel.activeSelf)
        {
            return false;
        }

        CancelConfirmation();
        return true;
    }

    private void BuildMainActionButtons()
    {
        mainActionButtons = new[]
        {
            startButton,
            continueButton,
            abandonButton,
            settingsButton,
            statsButton,
            exitButton
        };
    }

    private void CaptureSelectedTagOffset()
    {
        if (selectedTag == null || startButton == null || selectedTag.parent == null)
        {
            return;
        }

        Transform tagParent = selectedTag.parent;
        RectTransform startAnchor = GetMainActionAnchor(startButton);
        Vector3 startPosition = tagParent.InverseTransformPoint(startAnchor.position);
        selectedTagInitialLocalPosition = selectedTag.localPosition;
        selectedTagOffset = selectedTag.localPosition - startPosition;
        hasSelectedTagOffset = true;

        Graphic tagGraphic = selectedTag.GetComponent<Graphic>();
        if (tagGraphic != null)
        {
            tagGraphic.raycastTarget = false;
        }
    }

    private void InitializeMainActionSelection()
    {
        if (mainActionButtons == null || mainActionButtons.Length == 0)
        {
            BuildMainActionButtons();
        }

        Button preferredButton = GameSessionState.HasActiveRun && IsMainActionAvailable(continueButton)
            ? continueButton
            : startButton;
        selectedMainActionIndex = GetMainActionIndex(preferredButton);
        if (selectedMainActionIndex < 0)
        {
            selectedMainActionIndex = FindAvailableMainActionIndex(0, 1);
        }

        hasLastMainActionMousePosition = true;
        lastMainActionMousePosition = Input.mousePosition;
        ApplyMainActionSelection();
    }

    private void HandleMainActionInput()
    {
        UpdateMainActionSelectionFromMouse();
        RefreshSelectedTagPosition();

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveMainActionSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveMainActionSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateSelectedMainAction();
        }
    }

    private void UpdateMainActionSelectionFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (hasLastMainActionMousePosition
            && (mousePosition - lastMainActionMousePosition).sqrMagnitude < 0.01f)
        {
            return;
        }

        hasLastMainActionMousePosition = true;
        lastMainActionMousePosition = mousePosition;

        if (EventSystem.current != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = mousePosition
            };
            mainActionRaycastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, mainActionRaycastResults);
            for (int resultIndex = 0; resultIndex < mainActionRaycastResults.Count; resultIndex++)
            {
                Button hoveredButton = mainActionRaycastResults[resultIndex].gameObject.GetComponentInParent<Button>();
                int hoveredIndex = GetMainActionIndex(hoveredButton);
                if (hoveredIndex < 0 || !IsMainActionAvailable(hoveredButton))
                {
                    continue;
                }

                selectedMainActionIndex = hoveredIndex;
                ApplyMainActionSelection();
                return;
            }
        }

        Camera eventCamera = GetMainActionEventCamera();

        for (int index = 0; index < mainActionButtons.Length; index++)
        {
            Button button = mainActionButtons[index];
            RectTransform buttonRect = button != null ? button.transform as RectTransform : null;
            if (!IsMainActionAvailable(button)
                || buttonRect == null
                || !RectTransformUtility.RectangleContainsScreenPoint(buttonRect, mousePosition, eventCamera))
            {
                continue;
            }

            selectedMainActionIndex = index;
            ApplyMainActionSelection();
            return;
        }
    }

    private void MoveMainActionSelection(int direction)
    {
        if (mainActionButtons == null || mainActionButtons.Length == 0)
        {
            return;
        }

        int nextIndex = FindAvailableMainActionIndex(selectedMainActionIndex + direction, direction);
        if (nextIndex >= 0)
        {
            selectedMainActionIndex = nextIndex;
            ApplyMainActionSelection();
        }
    }

    private int FindAvailableMainActionIndex(int startIndex, int direction)
    {
        if (mainActionButtons == null || mainActionButtons.Length == 0)
        {
            return -1;
        }

        int step = direction < 0 ? -1 : 1;
        int index = ((startIndex % mainActionButtons.Length) + mainActionButtons.Length)
            % mainActionButtons.Length;
        for (int checkedCount = 0; checkedCount < mainActionButtons.Length; checkedCount++)
        {
            if (IsMainActionAvailable(mainActionButtons[index]))
            {
                return index;
            }

            index = (index + step + mainActionButtons.Length) % mainActionButtons.Length;
        }

        return -1;
    }

    private void ApplyMainActionSelection()
    {
        if (mainActionButtons == null
            || selectedMainActionIndex < 0
            || selectedMainActionIndex >= mainActionButtons.Length
            || !IsMainActionAvailable(mainActionButtons[selectedMainActionIndex]))
        {
            selectedMainActionIndex = FindAvailableMainActionIndex(0, 1);
        }

        Button selectedButton = selectedMainActionIndex >= 0
            ? mainActionButtons[selectedMainActionIndex]
            : null;
        UpdateSelectedTagPosition(selectedButton);
        selectedButton?.Select();
    }

    private void RefreshSelectedTagPosition()
    {
        Button selectedButton = mainActionButtons != null
            && selectedMainActionIndex >= 0
            && selectedMainActionIndex < mainActionButtons.Length
            ? mainActionButtons[selectedMainActionIndex]
            : null;
        UpdateSelectedTagPosition(selectedButton);
    }

    private void UpdateSelectedTagPosition(Button selectedButton)
    {
        if (selectedTag != null)
        {
            selectedTag.gameObject.SetActive(selectedButton != null);
            if (selectedButton != null && selectedTag.parent != null)
            {
                if (!hasSelectedTagOffset)
                {
                    CaptureSelectedTagOffset();
                }

                RectTransform buttonAnchor = GetMainActionAnchor(selectedButton);
                Vector3 buttonPosition = selectedTag.parent.InverseTransformPoint(buttonAnchor.position);
                Vector2 manualOffset = GetMainActionTagOffset(selectedButton);
                Vector3 tagPosition = selectedTagInitialLocalPosition;
                tagPosition.x = buttonPosition.x + selectedTagOffset.x + manualOffset.x;
                tagPosition.y = buttonPosition.y
                    + selectedTagOffset.y
                    + selectedTagVerticalAdjustment
                    + manualOffset.y;
                selectedTag.localPosition = tagPosition;
            }
        }
    }

    private void ActivateSelectedMainAction()
    {
        if (mainActionButtons == null
            || selectedMainActionIndex < 0
            || selectedMainActionIndex >= mainActionButtons.Length)
        {
            return;
        }

        Button selectedButton = mainActionButtons[selectedMainActionIndex];
        if (IsMainActionAvailable(selectedButton))
        {
            selectedButton.onClick.Invoke();
        }
    }

    private int GetMainActionIndex(Button button)
    {
        if (button == null || mainActionButtons == null)
        {
            return -1;
        }

        for (int index = 0; index < mainActionButtons.Length; index++)
        {
            if (mainActionButtons[index] == button)
            {
                return index;
            }
        }

        return -1;
    }

    private static RectTransform GetMainActionAnchor(Button button)
    {
        if (button != null && button.transform.childCount > 0)
        {
            RectTransform child = button.transform.GetChild(0) as RectTransform;
            if (child != null)
            {
                return child;
            }
        }

        return button != null ? button.transform as RectTransform : null;
    }

    private Vector2 GetMainActionTagOffset(Button button)
    {
        if (button == startButton)
        {
            return startTagOffset;
        }

        if (button == continueButton)
        {
            return continueTagOffset;
        }

        if (button == settingsButton)
        {
            return settingsTagOffset;
        }

        if (button == statsButton)
        {
            return statsTagOffset;
        }

        if (button == exitButton)
        {
            return exitTagOffset;
        }

        return Vector2.zero;
    }

    private void SelectMainAction(Button button)
    {
        int index = GetMainActionIndex(button);
        if (index < 0)
        {
            return;
        }

        selectedMainActionIndex = index;
        ApplyMainActionSelection();
    }

    private void SelectStartMainAction() => SelectMainAction(startButton);
    private void SelectContinueMainAction() => SelectMainAction(continueButton);
    private void SelectStatsMainAction() => SelectMainAction(statsButton);
    private void SelectSettingsMainAction() => SelectMainAction(settingsButton);
    private void SelectExitMainAction() => SelectMainAction(exitButton);

    private Camera GetMainActionEventCamera()
    {
        Canvas canvas = mainActionsPanel != null ? mainActionsPanel.GetComponentInParent<Canvas>() : null;
        return canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;
    }

    private static bool IsMainActionAvailable(Button button)
    {
        return button != null && button.gameObject.activeInHierarchy && button.interactable;
    }

    private void SelectSaveSlot1()
    {
        SelectSaveSlotFromMouse(0);
    }

    private void InitializeCharacterSelection()
    {
        int characterCount = navigationView != null ? navigationView.CharacterCount : 0;
        if (characterCount <= 0)
        {
            selectedCharacterIndex = 0;
            navigationView?.ShowCharacterSelected(0);
            return;
        }

        int savedCharacterIndex = CharacterCatalog.IndexOf(GameSessionState.CurrentCharacterId);
        selectedCharacterIndex = savedCharacterIndex >= 0 ? savedCharacterIndex : 0;
        ApplyCharacterSelection();
    }

    private void HandleCharacterSelectInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCharacterSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCharacterSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSelectedCharacter();
        }
    }

    private void SelectPreviousCharacter()
    {
        MoveCharacterSelection(-1);
    }

    private void SelectNextCharacter()
    {
        MoveCharacterSelection(1);
    }

    private void MoveCharacterSelection(int direction)
    {
        int characterCount = navigationView != null ? navigationView.CharacterCount : 0;
        if (characterCount <= 0)
        {
            return;
        }

        selectedCharacterIndex = (selectedCharacterIndex + direction + characterCount) % characterCount;
        ApplyCharacterSelection();
    }

    private void ApplyCharacterSelection()
    {
        navigationView?.ShowCharacterSelected(selectedCharacterIndex);
    }

    private void SelectSaveSlot2()
    {
        SelectSaveSlotFromMouse(1);
    }

    private void SelectSaveSlot3()
    {
        SelectSaveSlotFromMouse(2);
    }

    private void HandleSaveSelectInput()
    {
        UpdateSaveFocusFromMouse();

        if (deleteMode && Input.GetMouseButtonDown(1))
        {
            ExitDeleteMode();
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSaveSlotSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSaveSlotSelection(1);
        }

        if (!deleteMode && Input.GetKeyDown(KeyCode.W))
        {
            deleteSaveFocused = false;
            ApplySaveSelectFocus();
        }
        else if (!deleteMode && Input.GetKeyDown(KeyCode.S))
        {
            deleteSaveFocused = true;
            ApplySaveSelectFocus();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (deleteMode)
            {
                DeleteSelectedSaveImmediately();
            }
            else if (deleteSaveFocused)
            {
                EnterDeleteMode();
            }
            else
            {
                ConfirmSelectedSave();
            }
        }
    }

    private void UpdateSaveFocusFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (hasLastMousePosition && (mousePosition - lastMousePosition).sqrMagnitude < 0.01f)
        {
            return;
        }

        hasLastMousePosition = true;
        lastMousePosition = mousePosition;
        if (navigationView == null
            || !navigationView.TryGetHoveredSaveControl(mousePosition, out int slotIndex, out bool deleteHovered))
        {
            return;
        }

        if (slotIndex >= 0)
        {
            selectedSaveSlotIndex = slotIndex;
            if (!deleteMode)
            {
                deleteSaveFocused = false;
            }
        }
        else if (deleteHovered && !deleteMode)
        {
            deleteSaveFocused = true;
        }

        ApplySaveSelectFocus();
    }

    private void MoveSaveSlotSelection(int direction)
    {
        selectedSaveSlotIndex = (selectedSaveSlotIndex + direction + SaveContext.SlotCount)
            % SaveContext.SlotCount;
        if (!deleteMode)
        {
            deleteSaveFocused = false;
        }

        ApplySaveSelectFocus();
    }

    private void SelectSaveSlotFromMouse(int index)
    {
        selectedSaveSlotIndex = Mathf.Clamp(index, 0, SaveContext.SlotCount - 1);
        deleteSaveFocused = false;
        ApplySaveSelectFocus();
        if (deleteMode)
        {
            DeleteSelectedSaveImmediately();
        }
        else
        {
            ConfirmSelectedSave();
        }
    }

    private void ConfirmSelectedSave()
    {
        if (deleteMode)
        {
            DeleteSelectedSaveImmediately();
        }
        else
        {
            SelectSaveSlot(selectedSaveSlotIndex + 1);
        }
    }

    private void ToggleDeleteMode()
    {
        if (deleteMode)
        {
            ExitDeleteMode();
            return;
        }

        deleteSaveFocused = true;
        EnterDeleteMode();
    }

    private void EnterDeleteMode()
    {
        deleteMode = true;
        deleteSaveFocused = false;
        ApplySaveSelectFocus();
    }

    private void ExitDeleteMode()
    {
        deleteMode = false;
        deleteSaveFocused = false;
        ApplySaveSelectFocus();
    }

    private void DeleteSelectedSaveImmediately()
    {
        int slotId = selectedSaveSlotIndex + 1;
        SaveSlotInfo slot = SaveContext.GetSlotInfo(slotId);
        if (!slot.Exists)
        {
            navigationView?.SetSaveSelectStatus($"存档 {slotId} 为空，无需删除。", true);
            ApplySaveSelectFocus();
            return;
        }

        DeleteSaveSlot(slotId);
    }

    private void DeleteSaveSlot(int slotId)
    {
        if (slotId < 1 || slotId > SaveContext.SlotCount)
        {
            return;
        }

        if (SaveContext.CurrentSlotId == slotId)
        {
            GameSessionState.AbandonRun();
        }

        if (!SaveContext.DeleteSave(slotId))
        {
            navigationView?.SetSaveSelectStatus($"无法删除存档 {slotId}。", true);
            RefreshSaveSlots(false);
            return;
        }

        RefreshSaveSlots(false);
        RefreshSessionState();
        navigationView?.SetSaveSelectStatus($"已删除存档 {slotId}。", false);
    }

    private void SelectSaveSlot(int slotId)
    {
        if (!SaveContext.SelectOrCreateSave(slotId))
        {
            navigationView?.SetSaveSelectStatus($"无法读取存档 {slotId}，原文件未被覆盖。", true);
            RefreshSaveSlots(false);
            return;
        }

        RefreshSaveSlots();
        RefreshSessionState();
        uiRouter?.Navigate(UIRoute.MainMenu);
    }

    private void InitializeSaveSelect()
    {
        selectedSaveSlotIndex = SaveContext.CurrentSlotId > 0
            ? SaveContext.CurrentSlotId - 1
            : 0;
        selectedSaveSlotIndex = Mathf.Clamp(selectedSaveSlotIndex, 0, SaveContext.SlotCount - 1);
        deleteSaveFocused = false;
        deleteMode = false;
        hasLastMousePosition = true;
        lastMousePosition = Input.mousePosition;
        RefreshSaveSlots();
    }

    private void ApplySaveSelectFocus()
    {
        if (navigationView == null)
        {
            return;
        }

        navigationView.SetSaveSelection(selectedSaveSlotIndex, deleteSaveFocused, deleteMode);
    }

    private void RefreshSaveSlots(bool resetStatus = true)
    {
        if (navigationView == null)
        {
            return;
        }

        for (int index = 0; index < SaveContext.SlotCount; index++)
        {
            int slotId = index + 1;
            SaveSlotInfo slot = SaveContext.GetSlotInfo(slotId);
            navigationView.SetSaveSlotPresentation(index, slot);
        }

        ApplySaveSelectFocus();

        if (resetStatus)
        {
            navigationView.SetSaveSelectStatus("选择已有存档，或创建一个新存档", false);
        }
    }

    private void BindActions()
    {
        AddListener(navigationView != null ? navigationView.TitleContinueButton : null, OpenSaveSelect);
        AddListener(navigationView != null ? navigationView.GetSaveSlotButton(0) : null, SelectSaveSlot1);
        AddListener(navigationView != null ? navigationView.GetSaveSlotButton(1) : null, SelectSaveSlot2);
        AddListener(navigationView != null ? navigationView.GetSaveSlotButton(2) : null, SelectSaveSlot3);
        AddListener(navigationView != null ? navigationView.SaveSelectConfirmButton : null, ConfirmSelectedSave);
        AddListener(navigationView != null ? navigationView.DeleteFileButton : null, ToggleDeleteMode);
        AddListener(navigationView != null ? navigationView.SaveSelectBackButton : null, CloseSaveSelect);
        AddListener(navigationView != null ? navigationView.CharacterPreviousButton : null, SelectPreviousCharacter);
        AddListener(navigationView != null ? navigationView.CharacterNextButton : null, SelectNextCharacter);
        AddListener(navigationView != null ? navigationView.CharacterStartButton : null, StartSelectedCharacter);
        AddListener(navigationView != null ? navigationView.CharacterBackButton : null, CloseCharacterSelect);
        AddListener(startButton, SelectStartMainAction);
        AddListener(continueButton, SelectContinueMainAction);
        AddListener(statsButton, SelectStatsMainAction);
        AddListener(settingsButton, SelectSettingsMainAction);
        AddListener(exitButton, SelectExitMainAction);
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
        RemoveListener(navigationView != null ? navigationView.TitleContinueButton : null, OpenSaveSelect);
        RemoveListener(navigationView != null ? navigationView.GetSaveSlotButton(0) : null, SelectSaveSlot1);
        RemoveListener(navigationView != null ? navigationView.GetSaveSlotButton(1) : null, SelectSaveSlot2);
        RemoveListener(navigationView != null ? navigationView.GetSaveSlotButton(2) : null, SelectSaveSlot3);
        RemoveListener(navigationView != null ? navigationView.SaveSelectConfirmButton : null, ConfirmSelectedSave);
        RemoveListener(navigationView != null ? navigationView.DeleteFileButton : null, ToggleDeleteMode);
        RemoveListener(navigationView != null ? navigationView.SaveSelectBackButton : null, CloseSaveSelect);
        RemoveListener(navigationView != null ? navigationView.CharacterPreviousButton : null, SelectPreviousCharacter);
        RemoveListener(navigationView != null ? navigationView.CharacterNextButton : null, SelectNextCharacter);
        RemoveListener(navigationView != null ? navigationView.CharacterStartButton : null, StartSelectedCharacter);
        RemoveListener(navigationView != null ? navigationView.CharacterBackButton : null, CloseCharacterSelect);
        RemoveListener(startButton, SelectStartMainAction);
        RemoveListener(continueButton, SelectContinueMainAction);
        RemoveListener(statsButton, SelectStatsMainAction);
        RemoveListener(settingsButton, SelectSettingsMainAction);
        RemoveListener(exitButton, SelectExitMainAction);
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
