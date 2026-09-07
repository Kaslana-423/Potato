using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MainMenuFlowView : MonoBehaviour
{
    [Header("Title Visuals")]
    [SerializeField] private GameObject firstImage;
    [SerializeField] private GameObject secondImage;
    [SerializeField] private GameObject pressStart;
    [SerializeField] private GameObject brandArea;

    [Header("Pages")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private Button titleContinueButton;
    [SerializeField] private GameObject saveSelectPanel;
    [SerializeField] private TMP_Text saveSelectStatusText;
    [SerializeField] private Button[] saveSlotButtons;
    [SerializeField] private GameObject deleteFileRoot;
    [SerializeField] private GameObject deleteSelectedVisual;
    [SerializeField] private GameObject deleteModeVisual;
    [SerializeField] private Button deleteFileButton;
    [SerializeField] private Button saveSelectConfirmButton;
    [SerializeField] private Button saveSelectBackButton;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private TMP_Text characterSelectionStatusText;
    [SerializeField] private Button characterStartButton;
    [SerializeField] private Button characterBackButton;

    [Header("Character Selection")]
    [SerializeField] private Image characterPortraitImage;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text characterTypeText;
    [SerializeField] private TMP_Text characterWeaponText;
    [SerializeField] private TMP_Text characterDescriptionText;
    [SerializeField] private Button characterPreviousButton;
    [SerializeField] private Button characterNextButton;
    [SerializeField] private Image[] characterPageDots;
    [SerializeField] private Sprite selectedCharacterPageDotSprite;
    [SerializeField] private Sprite unselectedCharacterPageDotSprite;
    [SerializeField, Min(0f)] private float characterPageDotSpacing = 38f;

    private Sprite fallbackCharacterPortraitSprite;

    public GameObject TitlePanel => titlePanel;
    public Button TitleContinueButton => titleContinueButton;
    public GameObject SaveSelectPanel => saveSelectPanel;
    public Button DeleteFileButton => deleteFileButton;
    public Button SaveSelectConfirmButton => saveSelectConfirmButton;
    public Button SaveSelectBackButton => saveSelectBackButton;
    public GameObject CharacterSelectPanel => characterSelectPanel;
    public Button CharacterStartButton => characterStartButton;
    public Button CharacterBackButton => characterBackButton;
    public Button CharacterPreviousButton => characterPreviousButton;
    public Button CharacterNextButton => characterNextButton;
    public int CharacterCount => CharacterCatalog.All.Count;
    public Button FirstSaveSlotButton => saveSlotButtons != null && saveSlotButtons.Length > 0
        ? saveSlotButtons[0]
        : null;

    public void AutoBindSceneVisuals()
    {
        firstImage = firstImage != null ? firstImage : FindObject("First_Image");
        secondImage = secondImage != null ? secondImage : FindObject("Second_Image");
        pressStart = pressStart != null ? pressStart : FindObject("PRESS START");
        brandArea = brandArea != null ? brandArea : FindObject("BrandArea");
        saveSelectPanel = saveSelectPanel != null ? saveSelectPanel : FindObject("SaveSelectPanel");
        deleteFileRoot = deleteFileRoot != null ? deleteFileRoot : FindObject("DeleteFileButton");
        deleteSelectedVisual = deleteSelectedVisual != null
            ? deleteSelectedVisual
            : FindDescendant(deleteFileRoot != null ? deleteFileRoot.transform : null, "SELECTED")?.gameObject;
        deleteModeVisual = deleteModeVisual != null
            ? deleteModeVisual
            : FindDescendant(deleteFileRoot != null ? deleteFileRoot.transform : null, "DeleteMode")?.gameObject;
        deleteFileButton = deleteFileButton != null ? deleteFileButton : FindButton("DeleteFileButton");
        saveSelectConfirmButton = saveSelectConfirmButton != null
            ? saveSelectConfirmButton
            : FindButton("SelectButton");
        saveSelectBackButton = saveSelectBackButton != null
            ? saveSelectBackButton
            : FindButton("BackButton", "SaveSelectBackButton");
        characterSelectPanel = characterSelectPanel != null
            ? characterSelectPanel
            : FindObject("CharacterSelectPanel");
        characterSelectionStatusText = characterSelectionStatusText != null
            ? characterSelectionStatusText
            : FindComponent<TMP_Text>("CharacterSelectionStatusText");
        characterStartButton = characterStartButton != null
            ? characterStartButton
            : FindButton("CharacterStartButton");
        characterBackButton = characterBackButton != null
            ? characterBackButton
            : FindButton("CharacterBackButton");
        Image namedCharacterImage = FindDescendant(
            characterSelectPanel != null ? characterSelectPanel.transform : null,
            "Character")?.GetComponent<Image>();
        if (namedCharacterImage != null)
        {
            characterPortraitImage = namedCharacterImage;
        }
        if (characterPortraitImage != null && fallbackCharacterPortraitSprite == null)
        {
            fallbackCharacterPortraitSprite = characterPortraitImage.sprite;
        }
        characterNameText = characterNameText != null
            ? characterNameText
            : FindComponent<TMP_Text>("CharacterNameText");
        characterTypeText = characterTypeText != null
            ? characterTypeText
            : FindComponent<TMP_Text>("CharacterTypeText");
        characterWeaponText = characterWeaponText != null
            ? characterWeaponText
            : FindComponent<TMP_Text>("CharacterWeaponText");
        characterDescriptionText = characterDescriptionText != null
            ? characterDescriptionText
            : FindComponent<TMP_Text>("CharacterDescriptionText");
        characterPreviousButton = characterPreviousButton != null
            ? characterPreviousButton
            : FindButton("CharacterPreviousButton");
        characterNextButton = characterNextButton != null
            ? characterNextButton
            : FindButton("CharacterNextButton");
        BindCharacterPageDots();
        BindCharacterPageDotSprites();

        if (saveSlotButtons == null || saveSlotButtons.Length != SaveContext.SlotCount)
        {
            saveSlotButtons = new Button[SaveContext.SlotCount];
        }

        for (int index = 0; index < saveSlotButtons.Length; index++)
        {
            if (saveSlotButtons[index] == null)
            {
                saveSlotButtons[index] = FindButton($"SaveSlot{index + 1}Button");
            }
        }
    }

    public void ApplyRouteVisuals(UIRoute route)
    {
        if (route == UIRoute.None)
        {
            return;
        }

        bool showTitleVisuals = route == UIRoute.Title;
        SetActive(firstImage, showTitleVisuals);
        SetActive(titlePanel, showTitleVisuals);
        SetActive(pressStart, showTitleVisuals);
        SetActive(brandArea, showTitleVisuals);
        SetActive(secondImage, !showTitleVisuals);
    }

    public Button GetSaveSlotButton(int index)
    {
        return saveSlotButtons != null && index >= 0 && index < saveSlotButtons.Length
            ? saveSlotButtons[index]
            : null;
    }

    public void SetSaveSlotPresentation(int index, SaveSlotInfo slot)
    {
        Button button = GetSaveSlotButton(index);
        if (button == null)
        {
            return;
        }

        SetActive(FindDescendant(button.transform, "FileImage")?.gameObject, slot.Exists);
        SetActive(FindDescendant(button.transform, "NoFileImage")?.gameObject, !slot.Exists);
    }

    public void SetSaveSelection(int selectedIndex, bool deleteFocused, bool deleteMode)
    {
        for (int index = 0; index < SaveContext.SlotCount; index++)
        {
            Button button = GetSaveSlotButton(index);
            if (button == null)
            {
                continue;
            }

            bool selected = (deleteMode || !deleteFocused) && index == selectedIndex;
            SetActive(FindDescendant(button.transform, "Selected")?.gameObject, selected);
            SetActive(FindDescendant(button.transform, "Unselected")?.gameObject, !selected);
        }

        if (deleteFileButton != null)
        {
            deleteFileButton.interactable = true;
        }

        SetActive(deleteSelectedVisual, deleteFocused);
        SetActive(deleteModeVisual, deleteMode);

        Button focusedButton = deleteFocused && !deleteMode
            ? deleteFileButton
            : GetSaveSlotButton(selectedIndex);
        focusedButton?.Select();
    }

    public bool TryGetHoveredSaveControl(Vector2 screenPosition, out int slotIndex, out bool deleteHovered)
    {
        slotIndex = -1;
        deleteHovered = false;
        Camera eventCamera = GetEventCamera();

        for (int index = 0; index < SaveContext.SlotCount; index++)
        {
            Button button = GetSaveSlotButton(index);
            if (button != null
                && button.gameObject.activeInHierarchy
                && RectTransformUtility.RectangleContainsScreenPoint(button.transform as RectTransform, screenPosition, eventCamera))
            {
                slotIndex = index;
                return true;
            }
        }

        RectTransform deleteButtonRect = deleteFileButton != null
            ? deleteFileButton.transform as RectTransform
            : null;
        RectTransform deleteRootRect = deleteFileRoot != null
            ? deleteFileRoot.transform as RectTransform
            : null;
        bool hoveringDeleteButton = deleteButtonRect != null
            && deleteButtonRect.gameObject.activeInHierarchy
            && RectTransformUtility.RectangleContainsScreenPoint(
                deleteButtonRect,
                screenPosition,
                eventCamera);
        bool hoveringDeleteRoot = deleteRootRect != null
            && deleteRootRect.gameObject.activeInHierarchy
            && RectTransformUtility.RectangleContainsScreenPoint(
                deleteRootRect,
                screenPosition,
                eventCamera);
        if (hoveringDeleteButton || hoveringDeleteRoot)
        {
            deleteHovered = true;
            return true;
        }

        return false;
    }

    private static void SetActive(GameObject target, bool value)
    {
        if (target != null && target.activeSelf != value)
        {
            target.SetActive(value);
        }
    }

    private Camera GetEventCamera()
    {
        Canvas canvas = saveSelectPanel != null ? saveSelectPanel.GetComponentInParent<Canvas>() : null;
        return canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;
    }

#if UNITY_EDITOR
    public void EnsurePages(GameObject mainActionsPanel, TMP_FontAsset font)
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Main menu navigation pages must be initialized in the scene before entering Play Mode.", this);
            return;
        }

        AutoBindSceneVisuals();
        titlePanel = titlePanel != null ? titlePanel : FindObject("TitlePanel");
        saveSelectPanel = saveSelectPanel != null ? saveSelectPanel : FindObject("SaveSelectPanel");
        characterSelectPanel = characterSelectPanel != null
            ? characterSelectPanel
            : FindObject("CharacterSelectPanel");

        if (titlePanel == null)
        {
            CreateTitlePanel(mainActionsPanel, font);
        }

        if (saveSelectPanel == null)
        {
            CreateSaveSelectPanel(mainActionsPanel, font);
        }

        if (characterSelectPanel == null)
        {
            CreateCharacterSelectPanel(mainActionsPanel, font);
        }

        titleContinueButton = titleContinueButton != null
            ? titleContinueButton
            : FindComponent<Button>("TitleContinueButton");
        saveSelectStatusText = saveSelectStatusText != null
            ? saveSelectStatusText
            : FindComponent<TMP_Text>("SaveSelectStatusText");
        saveSelectBackButton = saveSelectBackButton != null
            ? saveSelectBackButton
            : FindComponent<Button>("SaveSelectBackButton");
        characterSelectionStatusText = characterSelectionStatusText != null
            ? characterSelectionStatusText
            : FindComponent<TMP_Text>("CharacterSelectionStatusText");
        characterStartButton = characterStartButton != null
            ? characterStartButton
            : FindComponent<Button>("CharacterStartButton");
        characterBackButton = characterBackButton != null
            ? characterBackButton
            : FindComponent<Button>("CharacterBackButton");

        if (saveSlotButtons == null || saveSlotButtons.Length != SaveContext.SlotCount)
        {
            saveSlotButtons = new Button[SaveContext.SlotCount];
        }

        for (int index = 0; index < saveSlotButtons.Length; index++)
        {
            if (saveSlotButtons[index] == null)
            {
                saveSlotButtons[index] = FindComponent<Button>($"SaveSlot{index + 1}Button");
            }
        }

        titlePanel.SetActive(false);
        saveSelectPanel.SetActive(false);
        characterSelectPanel.SetActive(false);
    }
#endif

    public void SetSaveSelectStatus(string value, bool isError)
    {
        if (saveSelectStatusText == null)
        {
            return;
        }

        saveSelectStatusText.text = value;
        saveSelectStatusText.color = isError
            ? new Color(0.95f, 0.35f, 0.32f, 1f)
            : new Color(0.63f, 0.69f, 0.76f, 1f);
    }

    public CharacterDefinition ShowCharacterSelected(int index)
    {
        IReadOnlyList<CharacterDefinition> characters = CharacterCatalog.All;
        if (characters.Count == 0)
        {
            SetText(characterNameText, "未配置角色");
            SetText(characterTypeText, "待配置");
            SetText(characterWeaponText, "初始武器：未配置");
            SetText(characterDescriptionText, "请在 Resources/Characters 中创建角色定义。");
            SetCharacterPortrait(null);
            SetCharacterPageDots(0, 0);
            SetCharacterNavigationState(0, false);
            return null;
        }

        int normalizedIndex = (index % characters.Count + characters.Count) % characters.Count;
        CharacterDefinition character = characters[normalizedIndex];
        SetText(characterNameText, character.DisplayName);
        SetText(characterTypeText, character.TypeLabel);
        SetText(characterWeaponText, $"初始武器：{ResolveStartingWeaponName(character)}");
        SetText(characterDescriptionText, character.Description);
        SetCharacterPortrait(character.Portrait);
        SetCharacterPageDots(characters.Count, normalizedIndex);
        SetCharacterNavigationState(characters.Count, character.Unlocked);

        if (characterSelectionStatusText != null)
        {
            characterSelectionStatusText.text = character.Unlocked ? string.Empty : "角色尚未解锁";
            characterSelectionStatusText.color = character.Unlocked ? AccentColor : MutedColor;
        }

        return character;
    }

    public CharacterDefinition GetCharacter(int index)
    {
        IReadOnlyList<CharacterDefinition> characters = CharacterCatalog.All;
        return index >= 0 && index < characters.Count ? characters[index] : null;
    }

    private void BindCharacterPageDots()
    {
        if (characterPageDots != null && characterPageDots.Length > 0)
        {
            return;
        }

        var dots = new List<Image>();
        for (int index = 1; ; index++)
        {
            Image dot = FindComponent<Image>($"CharacterPageDot{index}");
            if (dot == null)
            {
                break;
            }

            dots.Add(dot);
        }

        characterPageDots = dots.ToArray();
    }

    private void BindCharacterPageDotSprites()
    {
        if (selectedCharacterPageDotSprite == null
            && characterPageDots != null
            && characterPageDots.Length > 0
            && characterPageDots[0] != null)
        {
            selectedCharacterPageDotSprite = characterPageDots[0].sprite;
        }

        if (unselectedCharacterPageDotSprite == null)
        {
            unselectedCharacterPageDotSprite = Resources.Load<Sprite>("UI/big_roundframe");
        }

        if (unselectedCharacterPageDotSprite == null
            && characterPageDots != null
            && characterPageDots.Length > 1
            && characterPageDots[1] != null)
        {
            unselectedCharacterPageDotSprite = characterPageDots[1].sprite;
        }
    }

    private void SetCharacterPageDots(int characterCount, int selectedIndex)
    {
        BindCharacterPageDots();
        BindCharacterPageDotSprites();
        int availableDotCount = characterPageDots != null ? characterPageDots.Length : 0;
        int visibleDotCount = Mathf.Min(characterCount, availableDotCount);
        float firstX = -0.5f * (visibleDotCount - 1) * characterPageDotSpacing;

        for (int index = 0; index < availableDotCount; index++)
        {
            Image dot = characterPageDots[index];
            if (dot == null)
            {
                continue;
            }

            bool visible = index < visibleDotCount;
            SetActive(dot.gameObject, visible);
            if (!visible)
            {
                continue;
            }

            dot.sprite = index == selectedIndex
                ? selectedCharacterPageDotSprite
                : unselectedCharacterPageDotSprite;

            RectTransform dotRect = dot.rectTransform;
            dotRect.anchorMin = new Vector2(0.5f, 0.263f);
            dotRect.anchorMax = dotRect.anchorMin;
            dotRect.pivot = new Vector2(0.5f, 0.5f);
            dotRect.sizeDelta = new Vector2(20f, 20f);
            dotRect.anchoredPosition = new Vector2(firstX + index * characterPageDotSpacing, 0f);
        }
    }

    private void SetCharacterNavigationState(int characterCount, bool selectedCharacterUnlocked)
    {
        bool canChangeCharacter = characterCount > 1;
        if (characterPreviousButton != null)
        {
            characterPreviousButton.interactable = canChangeCharacter;
        }

        if (characterNextButton != null)
        {
            characterNextButton.interactable = canChangeCharacter;
        }

        if (characterStartButton != null)
        {
            characterStartButton.interactable = characterCount > 0 && selectedCharacterUnlocked;
        }
    }

    private void SetCharacterPortrait(Sprite portrait)
    {
        if (characterPortraitImage == null)
        {
            return;
        }

        characterPortraitImage.sprite = portrait != null ? portrait : fallbackCharacterPortraitSprite;
    }

    private static string ResolveStartingWeaponName(CharacterDefinition character)
    {
        if (!string.IsNullOrWhiteSpace(character.StartingWeaponDisplayName))
        {
            return character.StartingWeaponDisplayName;
        }

        ShopContentDefinition content = ShopContentCatalog.FindById(character.StartingWeaponId);
        return content != null ? content.LocalizedDisplayName : character.StartingWeaponId;
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
        {
            target.text = value ?? string.Empty;
        }
    }

#if UNITY_EDITOR
    private void CreateTitlePanel(GameObject templatePanel, TMP_FontAsset font)
    {
        titlePanel = CreatePage("TitlePanel", templatePanel);
        CreateLayoutText("TitleGameNameText", titlePanel.transform, "POTATO SURVIVOR", 54f, 150f, Color.white, font);
        CreateLayoutText("TitlePromptText", titlePanel.transform, "选择存档后开始游戏", 24f, 70f, MutedColor, font);
        CreateSpacer(titlePanel.transform, 70f);
        titleContinueButton = CreateButton("TitleContinueButton", titlePanel.transform, "进入", AccentColor, DarkColor, font, 78f);
    }

    private void CreateSaveSelectPanel(GameObject templatePanel, TMP_FontAsset font)
    {
        saveSelectPanel = CreatePage("SaveSelectPanel", templatePanel);
        CreateLayoutText("SaveSelectHeadingText", saveSelectPanel.transform, "选择存档", 40f, 68f, Color.white, font);
        saveSelectStatusText = CreateLayoutText(
            "SaveSelectStatusText",
            saveSelectPanel.transform,
            "选择已有存档，或创建一个新存档",
            20f,
            50f,
            MutedColor,
            font);

        saveSlotButtons = new Button[SaveContext.SlotCount];
        for (int index = 0; index < saveSlotButtons.Length; index++)
        {
            saveSlotButtons[index] = CreateButton(
                $"SaveSlot{index + 1}Button",
                saveSelectPanel.transform,
                $"存档 {index + 1}\n新存档",
                PanelLightColor,
                Color.white,
                font,
                88f);
        }

        CreateSpacer(saveSelectPanel.transform, 8f);
        saveSelectBackButton = CreateButton(
            "SaveSelectBackButton",
            saveSelectPanel.transform,
            "返回",
            PanelLightColor,
            Color.white,
            font,
            68f);
    }

    private void CreateCharacterSelectPanel(GameObject templatePanel, TMP_FontAsset font)
    {
        characterSelectPanel = CreatePage("CharacterSelectPanel", templatePanel);
        CreateLayoutText(
            "CharacterSelectHeadingText",
            characterSelectPanel.transform,
            "选择角色",
            40f,
            68f,
            Color.white,
            font);
        characterSelectionStatusText = CreateLayoutText(
            "CharacterSelectionStatusText",
            characterSelectPanel.transform,
            "当前仅开放默认角色",
            20f,
            50f,
            MutedColor,
            font);
        characterPortraitImage = CreateLayoutImage("Character", characterSelectPanel.transform, 130f);
        CreateLayoutText(
            "CharacterDescriptionText",
            characterSelectPanel.transform,
            "没有额外属性修正。适合熟悉移动、战斗和商店流程。",
            21f,
            90f,
            MutedColor,
            font);
        CreateSpacer(characterSelectPanel.transform, 18f);
        characterStartButton = CreateButton(
            "CharacterStartButton",
            characterSelectPanel.transform,
            "开始游戏",
            AccentColor,
            DarkColor,
            font,
            76f);
        characterBackButton = CreateButton(
            "CharacterBackButton",
            characterSelectPanel.transform,
            "返回",
            PanelLightColor,
            Color.white,
            font,
            68f);
        ShowCharacterSelected(0);
    }

    private GameObject CreatePage(string objectName, GameObject templatePanel)
    {
        GameObject page = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
        page.layer = gameObject.layer;
        page.transform.SetParent(transform, false);

        RectTransform rect = page.GetComponent<RectTransform>();
        RectTransform templateRect = templatePanel != null ? templatePanel.GetComponent<RectTransform>() : null;
        if (templateRect != null)
        {
            rect.anchorMin = templateRect.anchorMin;
            rect.anchorMax = templateRect.anchorMax;
            rect.anchoredPosition = templateRect.anchoredPosition;
            rect.sizeDelta = templateRect.sizeDelta;
            rect.pivot = templateRect.pivot;
        }
        else
        {
            rect.anchorMin = new Vector2(0.62f, 0.12f);
            rect.anchorMax = new Vector2(0.93f, 0.88f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        Image image = page.GetComponent<Image>();
        Image templateImage = templatePanel != null ? templatePanel.GetComponent<Image>() : null;
        image.color = templateImage != null ? templateImage.color : new Color(0.055f, 0.075f, 0.105f, 0.97f);

        VerticalLayoutGroup layout = page.GetComponent<VerticalLayoutGroup>();
        VerticalLayoutGroup templateLayout = templatePanel != null ? templatePanel.GetComponent<VerticalLayoutGroup>() : null;
        layout.padding = templateLayout != null
            ? new RectOffset(
                templateLayout.padding.left,
                templateLayout.padding.right,
                templateLayout.padding.top,
                templateLayout.padding.bottom)
            : new RectOffset(52, 52, 48, 48);
        layout.spacing = templateLayout != null ? templateLayout.spacing : 18f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        return page;
    }

    private static TMP_Text CreateLayoutText(
        string objectName,
        Transform parent,
        string value,
        float fontSize,
        float preferredHeight,
        Color color,
        TMP_FontAsset font)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(LayoutElement), typeof(TextMeshProUGUI));
        textObject.layer = parent.gameObject.layer;
        textObject.transform.SetParent(parent, false);
        textObject.GetComponent<LayoutElement>().preferredHeight = preferredHeight;

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
        text.fontStyle = FontStyles.Bold;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        if (font != null)
        {
            text.font = font;
        }

        return text;
    }

    private static Image CreateLayoutImage(string objectName, Transform parent, float preferredHeight)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(LayoutElement), typeof(Image));
        imageObject.layer = parent.gameObject.layer;
        imageObject.transform.SetParent(parent, false);
        imageObject.GetComponent<LayoutElement>().preferredHeight = preferredHeight;

        Image image = imageObject.GetComponent<Image>();
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return image;
    }

    private static Button CreateButton(
        string objectName,
        Transform parent,
        string labelValue,
        Color background,
        Color foreground,
        TMP_FontAsset font,
        float preferredHeight)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(Button));
        buttonObject.layer = parent.gameObject.layer;
        buttonObject.transform.SetParent(parent, false);
        buttonObject.GetComponent<LayoutElement>().preferredHeight = preferredHeight;

        Image image = buttonObject.GetComponent<Image>();
        image.color = background;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
        colors.pressedColor = new Color(0.78f, 0.82f, 0.86f, 1f);
        colors.disabledColor = new Color(0.38f, 0.4f, 0.43f, 0.55f);
        button.colors = colors;

        TMP_Text label = CreateLayoutText("Label", buttonObject.transform, labelValue, 26f, preferredHeight, foreground, font);
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(20f, 0f);
        labelRect.offsetMax = new Vector2(-20f, 0f);
        label.GetComponent<LayoutElement>().ignoreLayout = true;
        return button;
    }

    private static void CreateSpacer(Transform parent, float height)
    {
        GameObject spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
        spacer.layer = parent.gameObject.layer;
        spacer.transform.SetParent(parent, false);
        spacer.GetComponent<LayoutElement>().preferredHeight = height;
    }
#endif

    private GameObject FindObject(params string[] objectNames)
    {
        Transform child = FindTransform(objectNames);
        return child != null ? child.gameObject : null;
    }

    private T FindComponent<T>(params string[] objectNames) where T : Component
    {
        Transform child = FindTransform(objectNames);
        return child != null ? child.GetComponent<T>() : null;
    }

    private Button FindButton(params string[] objectNames)
    {
        Transform child = FindTransform(objectNames);
        if (child == null)
        {
            return null;
        }

        Button button = child.GetComponent<Button>();
        return button != null ? button : child.GetComponentInChildren<Button>(true);
    }

    private Transform FindTransform(params string[] objectNames)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in objectNames)
            {
                if (child.name == objectName)
                {
                    return child;
                }
            }
        }

        return null;
    }

    private static Transform FindDescendant(Transform parent, string objectName)
    {
        if (parent == null)
        {
            return null;
        }

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child != parent && child.name == objectName)
            {
                return child;
            }
        }

        return null;
    }

    private static readonly Color AccentColor = new Color(0.52f, 0.88f, 0.36f, 1f);
    private static readonly Color DarkColor = new Color(0.025f, 0.035f, 0.055f, 1f);
    private static readonly Color PanelLightColor = new Color(0.085f, 0.115f, 0.15f, 1f);
    private static readonly Color MutedColor = new Color(0.63f, 0.69f, 0.76f, 1f);
}
