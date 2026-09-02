using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MainMenuFlowView : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private Button titleContinueButton;
    [SerializeField] private GameObject saveSelectPanel;
    [SerializeField] private TMP_Text saveSelectStatusText;
    [SerializeField] private Button[] saveSlotButtons;
    [SerializeField] private Button saveSelectBackButton;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private TMP_Text characterSelectionStatusText;
    [SerializeField] private Button defaultCharacterButton;
    [SerializeField] private Button characterStartButton;
    [SerializeField] private Button characterBackButton;

    public GameObject TitlePanel => titlePanel;
    public Button TitleContinueButton => titleContinueButton;
    public GameObject SaveSelectPanel => saveSelectPanel;
    public Button SaveSelectBackButton => saveSelectBackButton;
    public GameObject CharacterSelectPanel => characterSelectPanel;
    public Button DefaultCharacterButton => defaultCharacterButton;
    public Button CharacterStartButton => characterStartButton;
    public Button CharacterBackButton => characterBackButton;
    public Button FirstSaveSlotButton => saveSlotButtons != null && saveSlotButtons.Length > 0
        ? saveSlotButtons[0]
        : null;

    public Button GetSaveSlotButton(int index)
    {
        return saveSlotButtons != null && index >= 0 && index < saveSlotButtons.Length
            ? saveSlotButtons[index]
            : null;
    }

#if UNITY_EDITOR
    public void EnsurePages(GameObject mainActionsPanel, TMP_FontAsset font)
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Main menu navigation pages must be initialized in the scene before entering Play Mode.", this);
            return;
        }

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
        defaultCharacterButton = defaultCharacterButton != null
            ? defaultCharacterButton
            : FindComponent<Button>("DefaultCharacterButton");
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

    public void SetSaveSlotLabel(int index, string value)
    {
        Button button = GetSaveSlotButton(index);
        TMP_Text label = button != null ? button.GetComponentInChildren<TMP_Text>(true) : null;
        if (label != null)
        {
            label.text = value;
        }
    }

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

    public void ShowDefaultCharacterSelected()
    {
        if (characterSelectionStatusText != null)
        {
            characterSelectionStatusText.text = "已选择：土豆　·　初始武器：木棍";
            characterSelectionStatusText.color = AccentColor;
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
        defaultCharacterButton = CreateButton(
            "DefaultCharacterButton",
            characterSelectPanel.transform,
            "土豆\n均衡的初始角色　·　木棍",
            PanelLightColor,
            Color.white,
            font,
            130f);
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
        ShowDefaultCharacterSelected();
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

    private GameObject FindObject(string objectName)
    {
        Transform child = FindTransform(objectName);
        return child != null ? child.gameObject : null;
    }

    private T FindComponent<T>(string objectName) where T : Component
    {
        Transform child = FindTransform(objectName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private Transform FindTransform(string objectName)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == objectName)
            {
                return child;
            }
        }

        return null;
    }
#endif

    private static readonly Color AccentColor = new Color(0.52f, 0.88f, 0.36f, 1f);
    private static readonly Color DarkColor = new Color(0.025f, 0.035f, 0.055f, 1f);
    private static readonly Color PanelLightColor = new Color(0.085f, 0.115f, 0.15f, 1f);
    private static readonly Color MutedColor = new Color(0.63f, 0.69f, 0.76f, 1f);
}
