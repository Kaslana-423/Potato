using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class CharacterSelectSceneAssembler
{
    private const string ScenePath = "Assets/Scenes/MainMenu.unity";
    private const string MarkerPath = "Assets/Editor/RunCharacterSelectAssembly.once";
    private const string CharacterAssetFolder = "Assets/Resources/Characters";
    private const string SelectedPageDotTexturePath = "Assets/Arts/1.png";
    private const string SelectedPageDotSpriteName = "1_7";
    private const string UnselectedPageDotSpritePath = "Assets/Resources/UI/big_roundframe.png";
    private const int CharacterPageDotCapacity = 12;

    static CharacterSelectSceneAssembler()
    {
        EditorApplication.update -= TryRunRequestedAssembly;
        EditorApplication.update += TryRunRequestedAssembly;
    }

    [MenuItem("Tools/Potato UI/Assemble Character Select Placeholder")]
    public static void AssembleFromMenu()
    {
        AssembleAndSave();
    }

    public static void AssembleFromCommandLine()
    {
        AssembleAndSave();
    }

    private static void TryRunRequestedAssembly()
    {
        if (!File.Exists(Path.GetFullPath(MarkerPath)))
        {
            EditorApplication.update -= TryRunRequestedAssembly;
            return;
        }

        if (EditorApplication.isCompiling
            || EditorApplication.isUpdating
            || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        try
        {
            AssembleAndSave();
            File.Delete(Path.GetFullPath(MarkerPath));
            string markerMetaPath = Path.GetFullPath(MarkerPath + ".meta");
            if (File.Exists(markerMetaPath))
            {
                File.Delete(markerMetaPath);
            }

            AssetDatabase.Refresh();
            EditorApplication.update -= TryRunRequestedAssembly;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private static void AssembleAndSave()
    {
        EnsureCharacterDefinitions();
        CharacterCatalog.Reload();

        Scene scene = SceneManager.GetSceneByPath(ScenePath);
        bool openedForAssembly = !scene.IsValid() || !scene.isLoaded;
        if (openedForAssembly)
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
        }

        try
        {
            MainMenuController controller = FindSceneComponent<MainMenuController>(scene);
            if (controller == null)
            {
                throw new InvalidOperationException("MainMenuController was not found in MainMenu.unity.");
            }

            MainMenuFlowView flowView = controller.GetComponent<MainMenuFlowView>();
            if (flowView == null)
            {
                throw new InvalidOperationException("MainMenuFlowView was not found in MainMenu.unity.");
            }

            flowView.AutoBindSceneVisuals();
            GameObject panel = flowView.CharacterSelectPanel;
            if (panel == null)
            {
                throw new InvalidOperationException("CharacterSelectPanel was not found in MainMenu.unity.");
            }

            TMP_Text heading = FindChild(panel.transform, "CharacterSelectHeadingText")?.GetComponent<TMP_Text>();
            TMP_FontAsset font = heading != null ? heading.font : TMP_Settings.defaultFontAsset;

            ConfigurePanel(panel);
            ConfigurePaperMasks(panel.transform);
            ConfigureHeading(panel.transform, heading, font);
            Image characterImage = ConfigureCharacterImage(panel.transform);
            ConfigureCharacterInformation(panel.transform, font);
            ConfigureCharacterNavigation(panel.transform, font);
            Button startButton = ConfigureStartButton(flowView.CharacterStartButton, panel.transform, font);
            Button backButton = ConfigureBackButton(panel.transform, font);
            ConfigureReferences(controller, flowView, panel, characterImage, startButton, backButton);

            Transform oldSpacer = FindDirectChild(panel.transform, "Spacer");
            if (oldSpacer != null)
            {
                oldSpacer.gameObject.SetActive(false);
            }

            EditorUtility.SetDirty(controller);
            EditorUtility.SetDirty(flowView);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
            {
                throw new IOException("MainMenu.unity could not be saved after character-select assembly.");
            }

            Debug.Log("Character Select placeholder layout assembled successfully.");
        }
        finally
        {
            if (openedForAssembly && scene.IsValid() && scene.isLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }

    private static void ConfigurePanel(GameObject panel)
    {
        Stretch(panel.GetComponent<RectTransform>());
        Image panelImage = EnsureImage(panel);
        panelImage.sprite = null;
        panelImage.color = Color.clear;
        panelImage.raycastTarget = false;

        foreach (LayoutGroup layoutGroup in panel.GetComponents<LayoutGroup>())
        {
            layoutGroup.enabled = false;
        }

        ContentSizeFitter contentSizeFitter = panel.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter != null)
        {
            contentSizeFitter.enabled = false;
        }
    }

    private static void ConfigurePaperMasks(Transform panel)
    {
        Transform paper = EnsureRectObject("CharacterPaperMask", panel);
        SetNormalized(paper as RectTransform, new Vector2(0.18f, 0.13f), new Vector2(0.82f, 0.87f));
        SetWhiteMask(paper.gameObject);
        paper.SetAsFirstSibling();

        Transform header = EnsureRectObject("CharacterHeaderMask", panel);
        SetNormalized(header as RectTransform, new Vector2(0.34f, 0.82f), new Vector2(0.66f, 0.96f));
        SetWhiteMask(header.gameObject);

        Transform leftPin = EnsureRectObject("HeaderLeftPinMask", panel);
        SetNormalized(leftPin as RectTransform, new Vector2(0.305f, 0.855f), new Vector2(0.335f, 0.915f));
        SetWhiteMask(leftPin.gameObject);

        Transform rightPin = EnsureRectObject("HeaderRightPinMask", panel);
        SetNormalized(rightPin as RectTransform, new Vector2(0.665f, 0.855f), new Vector2(0.695f, 0.915f));
        SetWhiteMask(rightPin.gameObject);
    }

    private static void ConfigureHeading(Transform panel, TMP_Text heading, TMP_FontAsset font)
    {
        heading = heading != null ? heading : EnsureText("CharacterSelectHeadingText", panel, font);
        heading.gameObject.SetActive(true);
        heading.text = "选择角色";
        heading.fontSize = 68f;
        heading.color = TextColor;
        heading.alignment = TextAlignmentOptions.Center;
        heading.raycastTarget = false;
        SetNormalized(heading.rectTransform, new Vector2(0.35f, 0.835f), new Vector2(0.65f, 0.945f));
        heading.transform.SetAsLastSibling();
    }

    private static Image ConfigureCharacterImage(Transform panel)
    {
        Transform character = FindDirectChild(panel, "Character");
        bool created = character == null;
        if (created)
        {
            character = EnsureRectObject("Character", panel);
            SetNormalized(character as RectTransform, new Vector2(0.235f, 0.315f), new Vector2(0.475f, 0.75f));
        }

        Image image = EnsureImage(character.gameObject);
        if (created)
        {
            image.sprite = null;
            image.color = Color.white;
            image.raycastTarget = false;
        }

        return image;
    }

    private static void ConfigureCharacterInformation(Transform panel, TMP_FontAsset font)
    {
        TMP_Text name = EnsureText("CharacterNameText", panel, font);
        ConfigureText(name, "土豆", 60f, TextAlignmentOptions.Center,
            new Vector2(0.50f, 0.65f), new Vector2(0.76f, 0.75f));

        Transform typeMask = EnsureRectObject("CharacterTypeMask", panel);
        SetNormalized(typeMask as RectTransform, new Vector2(0.525f, 0.575f), new Vector2(0.66f, 0.625f));
        SetWhiteMask(typeMask.gameObject);

        TMP_Text type = EnsureText("CharacterTypeText", panel, font);
        ConfigureText(type, "均衡型", 32f, TextAlignmentOptions.Center,
            new Vector2(0.525f, 0.575f), new Vector2(0.66f, 0.625f));

        TMP_Text weapon = EnsureText("CharacterWeaponText", panel, font);
        ConfigureText(weapon, "初始武器：木棍", 31f, TextAlignmentOptions.Left,
            new Vector2(0.515f, 0.495f), new Vector2(0.77f, 0.56f));

        Transform divider = EnsureRectObject("CharacterInfoDividerMask", panel);
        SetNormalized(divider as RectTransform, new Vector2(0.515f, 0.48f), new Vector2(0.77f, 0.485f));
        SetWhiteMask(divider.gameObject);

        TMP_Text description = FindChild(panel, "CharacterDescriptionText")?.GetComponent<TMP_Text>();
        description = description != null ? description : EnsureText("CharacterDescriptionText", panel, font);
        ConfigureText(description, "没有额外属性修正\n适合熟悉游戏流程", 28f, TextAlignmentOptions.TopLeft,
            new Vector2(0.515f, 0.35f), new Vector2(0.77f, 0.465f));
    }

    private static void ConfigureCharacterNavigation(Transform panel, TMP_FontAsset font)
    {
        Button previous = EnsureButton("CharacterPreviousButton", panel);
        ConfigureWhiteButton(previous, "<", font,
            new Vector2(0.365f, 0.225f), new Vector2(0.42f, 0.295f), 52f);

        Button next = EnsureButton("CharacterNextButton", panel);
        ConfigureWhiteButton(next, ">", font,
            new Vector2(0.58f, 0.225f), new Vector2(0.635f, 0.295f), 52f);

        Sprite selectedSprite = LoadSprite(SelectedPageDotTexturePath, SelectedPageDotSpriteName);
        Sprite unselectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(UnselectedPageDotSpritePath);
        int visibleDotCount = Mathf.Min(CharacterCatalog.All.Count, CharacterPageDotCapacity);
        for (int index = 0; index < CharacterPageDotCapacity; index++)
        {
            float centerX = 0.5f + (index - (visibleDotCount - 1) * 0.5f) * 0.024f;
            Transform dot = EnsureRectObject($"CharacterPageDot{index + 1}", panel);
            SetNormalized(dot as RectTransform,
                new Vector2(centerX - 0.006f, 0.252f),
                new Vector2(centerX + 0.006f, 0.274f));
            Image dotImage = EnsureImage(dot.gameObject);
            dotImage.sprite = index == 0 ? selectedSprite : unselectedSprite;
            dotImage.raycastTarget = false;
            dot.gameObject.SetActive(index < visibleDotCount);
        }
    }

    private static Button ConfigureStartButton(Button button, Transform panel, TMP_FontAsset font)
    {
        if (button == null)
        {
            button = EnsureButton("CharacterStartButton", panel);
        }

        ConfigureWhiteButton(button, "开始游戏", font,
            new Vector2(0.36f, 0.08f), new Vector2(0.64f, 0.19f), 50f);
        return button;
    }

    private static Button ConfigureBackButton(Transform panel, TMP_FontAsset font)
    {
        Button button = EnsureButton("CharacterBackButton", panel);
        ConfigureWhiteButton(button, "返回", font,
            new Vector2(0.43f, 0.01f), new Vector2(0.57f, 0.075f), 34f);
        return button;
    }

    private static void ConfigureReferences(
        MainMenuController controller,
        MainMenuFlowView flowView,
        GameObject panel,
        Image characterImage,
        Button startButton,
        Button backButton)
    {
        SerializedObject serializedFlow = new SerializedObject(flowView);
        serializedFlow.FindProperty("characterSelectPanel").objectReferenceValue = panel;
        serializedFlow.FindProperty("characterSelectionStatusText").objectReferenceValue = null;
        serializedFlow.FindProperty("characterStartButton").objectReferenceValue = startButton;
        serializedFlow.FindProperty("characterBackButton").objectReferenceValue = backButton;
        serializedFlow.FindProperty("characterPortraitImage").objectReferenceValue = characterImage;
        serializedFlow.FindProperty("characterNameText").objectReferenceValue = FindChild(panel.transform, "CharacterNameText")?.GetComponent<TMP_Text>();
        serializedFlow.FindProperty("characterTypeText").objectReferenceValue = FindChild(panel.transform, "CharacterTypeText")?.GetComponent<TMP_Text>();
        serializedFlow.FindProperty("characterWeaponText").objectReferenceValue = FindChild(panel.transform, "CharacterWeaponText")?.GetComponent<TMP_Text>();
        serializedFlow.FindProperty("characterDescriptionText").objectReferenceValue = FindChild(panel.transform, "CharacterDescriptionText")?.GetComponent<TMP_Text>();
        serializedFlow.FindProperty("characterPreviousButton").objectReferenceValue = FindChild(panel.transform, "CharacterPreviousButton")?.GetComponent<Button>();
        serializedFlow.FindProperty("characterNextButton").objectReferenceValue = FindChild(panel.transform, "CharacterNextButton")?.GetComponent<Button>();
        serializedFlow.FindProperty("selectedCharacterPageDotSprite").objectReferenceValue = LoadSprite(SelectedPageDotTexturePath, SelectedPageDotSpriteName);
        serializedFlow.FindProperty("unselectedCharacterPageDotSprite").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(UnselectedPageDotSpritePath);

        SerializedProperty pageDots = serializedFlow.FindProperty("characterPageDots");
        pageDots.arraySize = CharacterPageDotCapacity;
        for (int index = 0; index < CharacterPageDotCapacity; index++)
        {
            pageDots.GetArrayElementAtIndex(index).objectReferenceValue =
                FindChild(panel.transform, $"CharacterPageDot{index + 1}")?.GetComponent<Image>();
        }

        serializedFlow.ApplyModifiedPropertiesWithoutUndo();

        UIScreen screen = panel.GetComponent<UIScreen>();
        if (screen == null)
        {
            screen = panel.AddComponent<UIScreen>();
        }

        screen.Configure(UIRoute.CharacterSelect, panel, startButton);
        SerializedObject serializedController = new SerializedObject(controller);
        serializedController.FindProperty("characterSelectScreen").objectReferenceValue = screen;
        serializedController.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(screen);
    }

    private static void ConfigureWhiteButton(
        Button button,
        string labelValue,
        TMP_FontAsset font,
        Vector2 anchorMin,
        Vector2 anchorMax,
        float fontSize)
    {
        SetNormalized(button.GetComponent<RectTransform>(), anchorMin, anchorMax);
        Image image = EnsureImage(button.gameObject);
        image.sprite = null;
        image.color = Color.white;
        image.raycastTarget = true;
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        button.interactable = true;

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        label = label != null ? label : EnsureText("Label", button.transform, font);
        label.gameObject.SetActive(true);
        label.text = labelValue;
        label.fontSize = fontSize;
        label.color = TextColor;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        Stretch(label.rectTransform);
    }

    private static void EnsureCharacterDefinitions()
    {
        EnsureAssetFolder("Assets", "Resources");
        EnsureAssetFolder("Assets/Resources", "Characters");

        EnsureCharacterDefinition(
            "Potato",
            "character.potato",
            0,
            "土豆",
            "均衡型",
            "没有额外属性修正\n适合熟悉游戏流程",
            "weapon.stick.tier_1",
            "木棍",
            Array.Empty<PlayerStatId>(),
            Array.Empty<int>());
        EnsureCharacterDefinition(
            "Fighter",
            "character.fighter",
            1,
            "斗士",
            "近战型",
            "近战伤害 +5\n最大生命值 +5",
            "weapon.knife.tier_1",
            "小刀",
            new[] { PlayerStatId.MeleeDamage, PlayerStatId.MaxHp },
            new[] { 5, 5 });
        EnsureCharacterDefinition(
            "Ranger",
            "character.ranger",
            2,
            "游侠",
            "精准型",
            "攻击速度 +10\n攻击范围 +50\n最大生命值 -5",
            "weapon.spear.tier_1",
            "长矛",
            new[] { PlayerStatId.AttackSpeed, PlayerStatId.Range, PlayerStatId.MaxHp },
            new[] { 10, 50, -5 });
        EnsureCharacterDefinition(
            "LuckyStar",
            "character.lucky_star",
            3,
            "幸运星",
            "幸运型",
            "幸运 +15\n收获 +10\n伤害 -10",
            "weapon.rock.tier_1",
            "石头",
            new[] { PlayerStatId.Luck, PlayerStatId.Harvesting, PlayerStatId.Damage },
            new[] { 15, 10, -10 });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void EnsureCharacterDefinition(
        string assetName,
        string id,
        int displayOrder,
        string displayName,
        string typeLabel,
        string description,
        string startingWeaponId,
        string startingWeaponDisplayName,
        PlayerStatId[] statIds,
        int[] statAmounts)
    {
        string assetPath = $"{CharacterAssetFolder}/{assetName}.asset";
        CharacterDefinition definition = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(assetPath);
        if (definition != null)
        {
            return;
        }

        definition = ScriptableObject.CreateInstance<CharacterDefinition>();
        definition.name = assetName;
        AssetDatabase.CreateAsset(definition, assetPath);

        SerializedObject serializedDefinition = new SerializedObject(definition);
        serializedDefinition.FindProperty("id").stringValue = id;
        serializedDefinition.FindProperty("displayOrder").intValue = displayOrder;
        serializedDefinition.FindProperty("visibleInSelection").boolValue = true;
        serializedDefinition.FindProperty("unlocked").boolValue = true;
        serializedDefinition.FindProperty("displayName").stringValue = displayName;
        serializedDefinition.FindProperty("typeLabel").stringValue = typeLabel;
        serializedDefinition.FindProperty("description").stringValue = description;
        serializedDefinition.FindProperty("portrait").objectReferenceValue = null;
        serializedDefinition.FindProperty("startingWeaponId").stringValue = startingWeaponId;
        serializedDefinition.FindProperty("startingWeaponDisplayName").stringValue = startingWeaponDisplayName;

        SerializedProperty modifiers = serializedDefinition.FindProperty("startingStatModifiers");
        int modifierCount = Mathf.Min(statIds?.Length ?? 0, statAmounts?.Length ?? 0);
        modifiers.arraySize = modifierCount;
        for (int index = 0; index < modifierCount; index++)
        {
            SerializedProperty modifier = modifiers.GetArrayElementAtIndex(index);
            modifier.FindPropertyRelative("statId").enumValueIndex = (int)statIds[index];
            modifier.FindPropertyRelative("amount").intValue = statAmounts[index];
        }

        serializedDefinition.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(definition);
    }

    private static void EnsureAssetFolder(string parentFolder, string folderName)
    {
        string fullPath = $"{parentFolder}/{folderName}";
        if (!AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.CreateFolder(parentFolder, folderName);
        }
    }

    private static void ConfigureText(
        TMP_Text text,
        string value,
        float fontSize,
        TextAlignmentOptions alignment,
        Vector2 anchorMin,
        Vector2 anchorMax)
    {
        text.gameObject.SetActive(true);
        text.text = value;
        text.fontSize = fontSize;
        text.color = TextColor;
        text.alignment = alignment;
        text.raycastTarget = false;
        SetNormalized(text.rectTransform, anchorMin, anchorMax);
    }

    private static Button EnsureButton(string objectName, Transform parent)
    {
        Transform target = EnsureRectObject(objectName, parent);
        Button button = target.GetComponent<Button>();
        return button != null ? button : target.gameObject.AddComponent<Button>();
    }

    private static TMP_Text EnsureText(string objectName, Transform parent, TMP_FontAsset font)
    {
        Transform target = EnsureRectObject(objectName, parent);
        TMP_Text text = target.GetComponent<TMP_Text>();
        if (text == null)
        {
            text = target.gameObject.AddComponent<TextMeshProUGUI>();
        }

        if (text.font == null && font != null)
        {
            text.font = font;
        }

        return text;
    }

    private static Image SetWhiteMask(GameObject target)
    {
        Image image = EnsureImage(target);
        image.sprite = null;
        image.color = Color.white;
        image.raycastTarget = false;
        return image;
    }

    private static Image EnsureImage(GameObject target)
    {
        Image image = target.GetComponent<Image>();
        return image != null ? image : target.AddComponent<Image>();
    }

    private static Sprite LoadSprite(string assetPath, string spriteName)
    {
        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        for (int index = 0; index < assets.Length; index++)
        {
            if (assets[index] is Sprite sprite && sprite.name == spriteName)
            {
                return sprite;
            }
        }

        Debug.LogWarning($"Sprite '{spriteName}' was not found at '{assetPath}'.");
        return null;
    }

    private static Transform EnsureRectObject(string objectName, Transform parent)
    {
        Transform existing = FindDirectChild(parent, objectName);
        if (existing != null)
        {
            return existing;
        }

        GameObject created = new GameObject(objectName, typeof(RectTransform));
        created.layer = parent.gameObject.layer;
        created.transform.SetParent(parent, false);
        return created.transform;
    }

    private static Transform FindDirectChild(Transform parent, string objectName)
    {
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (child.name == objectName)
            {
                return child;
            }
        }

        return null;
    }

    private static Transform FindChild(Transform parent, string objectName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == objectName)
            {
                return child;
            }
        }

        return null;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
    }

    private static void SetNormalized(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            T component = rootObject.GetComponentInChildren<T>(true);
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }

    private static readonly Color TextColor = new Color32(86, 35, 30, 255);
}
