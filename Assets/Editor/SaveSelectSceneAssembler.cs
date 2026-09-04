using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class SaveSelectSceneAssembler
{
    private const string ScenePath = "Assets/Scenes/MainMenu.unity";
    private const string MarkerPath = "Assets/Editor/RunSaveSelectAssembly.once";
    private const string ArtRoot = "Assets/Arts/pohe2/";

    private static double nextRetryTime;

    static SaveSelectSceneAssembler()
    {
        EditorApplication.update -= TryRunRequestedAssembly;
        EditorApplication.update += TryRunRequestedAssembly;
    }

    [MenuItem("Tools/Potato UI/Assemble Save Select Artwork")]
    public static void AssembleFromMenu()
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

        if (EditorApplication.timeSinceStartup < nextRetryTime
            || EditorApplication.isCompiling
            || EditorApplication.isUpdating
            || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        nextRetryTime = EditorApplication.timeSinceStartup + 2d;
        Scene loadedScene = SceneManager.GetSceneByPath(ScenePath);
        if (loadedScene.IsValid() && loadedScene.isLoaded && loadedScene.isDirty)
        {
            Debug.Log("Save Select artwork assembly is waiting for MainMenu.unity to be saved.");
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
            GameObject savePanel = flowView.SaveSelectPanel;
            if (savePanel == null)
            {
                throw new InvalidOperationException("SaveSelectPanel was not found in MainMenu.unity.");
            }

            ConfigureBackground(controller.transform);
            ConfigurePanel(savePanel);
            ConfigureArtwork(savePanel.transform);
            ConfigureSaveSlot(flowView.GetSaveSlotButton(0), 0);
            ConfigureSaveSlot(flowView.GetSaveSlotButton(1), 1);
            ConfigureSaveSlot(flowView.GetSaveSlotButton(2), 2);
            ConfigureBottomButtons(flowView, savePanel.transform);
            ConfigureNavigationScreen(controller, flowView, savePanel);

            EditorUtility.SetDirty(flowView);
            EditorUtility.SetDirty(controller);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
            {
                throw new IOException("MainMenu.unity could not be saved after artwork assembly.");
            }

            Debug.Log("Save Select artwork and scene references were assembled successfully.");
        }
        finally
        {
            if (openedForAssembly && scene.IsValid() && scene.isLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }

    private static void ConfigureBackground(Transform controllerRoot)
    {
        Transform backgroundTransform = FindChild(controllerRoot, "Second_Image");
        if (backgroundTransform == null)
        {
            throw new InvalidOperationException("Second_Image was not found.");
        }

        Image background = EnsureImage(backgroundTransform.gameObject);
        background.sprite = LoadSprite(ArtRoot + "back2.png");
        background.color = Color.white;
        background.preserveAspect = false;
        background.raycastTarget = false;
        Stretch(background.rectTransform);
        EditorUtility.SetDirty(background);
    }

    private static void ConfigurePanel(GameObject savePanel)
    {
        Stretch(savePanel.GetComponent<RectTransform>());
        Image panelImage = savePanel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = Color.clear;
            panelImage.raycastTarget = false;
            EditorUtility.SetDirty(panelImage);
        }

        Transform heading = FindChild(savePanel.transform, "SaveSelectHeadingText");
        if (heading != null)
        {
            heading.gameObject.SetActive(false);
        }
    }

    private static void ConfigureArtwork(Transform panel)
    {
        Transform artwork = EnsureRectObject("SaveSelectArtwork", panel);
        Stretch(artwork as RectTransform);
        artwork.SetAsFirstSibling();
    }

    private static void ConfigureSaveSlot(Button button, int index)
    {
        if (button == null)
        {
            throw new InvalidOperationException($"Save slot button {index + 1} was not found.");
        }

        Vector2[] positions =
        {
            new Vector2(-595f, 85f),
            new Vector2(20f, 15f),
            new Vector2(625f, 10f)
        };
        Vector2[] sizes =
        {
            new Vector2(597f, 749f),
            new Vector2(543f, 714f),
            new Vector2(608f, 755f)
        };
        string[] papers =
        {
            ArtRoot + "组1/矢量智能对象-3.png",
            ArtRoot + "组1/矢量智能对象-6.png",
            ArtRoot + "组1/矢量智能对象-1.png"
        };
        string[] tapes =
        {
            ArtRoot + "组1/矢量智能对象-2.png",
            ArtRoot + "组1/矢量智能对象-5.png",
            ArtRoot + "组1/矢量智能对象.png"
        };
        Vector2[] tapePositions =
        {
            new Vector2(-75f, 335f),
            new Vector2(-135f, 330f),
            new Vector2(5f, 350f)
        };
        Vector2[] tapeSizes =
        {
            new Vector2(167f, 156f),
            new Vector2(269f, 176f),
            new Vector2(226f, 88f)
        };

        RectTransform buttonRect = button.GetComponent<RectTransform>();
        SetCentered(buttonRect, positions[index], sizes[index]);
        buttonRect.localScale = Vector3.one;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        Image hitGraphic = EnsureImage(button.gameObject);
        hitGraphic.sprite = null;
        hitGraphic.color = new Color(1f, 1f, 1f, 0.001f);
        hitGraphic.raycastTarget = true;
        button.targetGraphic = hitGraphic;

        Transform paper = EnsureRectObject("Paper", button.transform);
        Stretch(paper as RectTransform);
        Image paperImage = EnsureImage(paper.gameObject);
        paperImage.sprite = LoadSprite(papers[index]);
        paperImage.color = Color.white;
        paperImage.preserveAspect = true;
        paperImage.raycastTarget = false;
        paper.SetAsFirstSibling();

        Transform fileImage = RequireChild(button.transform, "FileImage");
        SetCentered(fileImage as RectTransform, new Vector2(index == 0 ? 15f : 0f, index == 0 ? 85f : 45f), new Vector2(328f, 329f));
        Image portrait = EnsureImage(fileImage.gameObject);
        portrait.sprite = LoadSprite(ArtRoot + "渐变映射 1.png");
        portrait.color = Color.white;
        portrait.preserveAspect = true;
        portrait.raycastTarget = false;

        Transform noFile = RequireChild(button.transform, "NoFileImage");
        Stretch(noFile as RectTransform);
        Transform shadow = EnsureRectObject("Shadow", noFile);
        ConfigureImage(shadow, ArtRoot + "组1/椭圆 1.png", new Vector2(0f, -110f), new Vector2(294f, 65f));
        Transform character = EnsureRectObject("Character", noFile);
        ConfigureImage(character, ArtRoot + "组1/图层 2.png", new Vector2(0f, 35f), new Vector2(363f, 328f));

        Transform selected = RequireChild(button.transform, "Selected");
        SetCentered(selected as RectTransform, new Vector2(0f, sizes[index].y * 0.45f), new Vector2(81f, 85f));
        Image selectedPin = EnsureImage(selected.gameObject);
        selectedPin.sprite = LoadSprite(index == 2
            ? ArtRoot + "矢量智能对象-1.png"
            : ArtRoot + "矢量智能对象.png");
        selectedPin.color = Color.white;
        selectedPin.preserveAspect = true;
        selectedPin.raycastTarget = false;

        Transform unselected = RequireChild(button.transform, "Unselected");
        Stretch(unselected as RectTransform);

        Transform fileWord = EnsureRectObject("FileWord", button.transform);
        ConfigureImage(fileWord, ArtRoot + "FILE.png", new Vector2(-42f, -280f), new Vector2(173f, 99f));

        TMP_Text label = FindChild(button.transform, "Label")?.GetComponent<TMP_Text>();
        if (label != null)
        {
            label.gameObject.SetActive(true);
            label.text = (index + 1).ToString();
            label.fontSize = 78f;
            label.fontStyle = FontStyles.Bold;
            label.color = new Color32(83, 27, 25, 255);
            label.alignment = TextAlignmentOptions.Center;
            SetCentered(label.rectTransform, new Vector2(92f, -278f), new Vector2(90f, 110f));
            label.raycastTarget = false;
            EditorUtility.SetDirty(label);
        }

        Transform tape = EnsureRectObject("Tape", button.transform);
        ConfigureImage(tape, tapes[index], tapePositions[index], tapeSizes[index]);
        tape.SetAsLastSibling();
        selected.SetAsLastSibling();


        EditorUtility.SetDirty(button);
        EditorUtility.SetDirty(buttonRect);
    }

    private static void ConfigureBottomButtons(MainMenuFlowView flowView, Transform panel)
    {
        Button backButton = flowView.SaveSelectBackButton;
        Button selectButton = flowView.SaveSelectConfirmButton;
        Button deleteButton = flowView.DeleteFileButton;
        if (backButton == null || selectButton == null || deleteButton == null)
        {
            throw new InvalidOperationException("Back, Select, or Delete button is missing from SaveSelectPanel.");
        }

        ConfigurePaperButton(
            backButton,
            ArtRoot + "矢量智能对象-2.png",
            new Vector2(-735f, -405f),
            new Vector2(491f, 298f),
            "BACK\nEsc");
        ConfigurePaperButton(
            selectButton,
            ArtRoot + "矢量智能对象-3.png",
            new Vector2(730f, -408f),
            new Vector2(461f, 287f),
            "Space");

        Transform selectWord = EnsureRectObject("SelectWord", selectButton.transform);
        ConfigureImage(selectWord, ArtRoot + "SELECT.png", new Vector2(0f, 38f), new Vector2(268f, 83f));

        Transform selectPin = EnsureRectObject("DecorativePin", selectButton.transform);
        ConfigureImage(selectPin, ArtRoot + "矢量智能对象-1.png", new Vector2(-45f, 142f), new Vector2(79f, 86f));
        selectPin.SetAsLastSibling();

        Transform deleteRoot = deleteButton.transform.parent;
        if (deleteRoot == null || deleteRoot.name != "DeleteFileButton")
        {
            deleteRoot = FindChild(panel, "DeleteFileButton");
        }

        if (deleteRoot == null)
        {
            throw new InvalidOperationException("DeleteFileButton root was not found.");
        }

        SetCentered(deleteRoot as RectTransform, new Vector2(0f, -445f), new Vector2(812f, 179f));
        Image deletePaper = EnsureImage(deleteRoot.gameObject);
        deletePaper.sprite = LoadSprite(ArtRoot + "组1/矢量智能对象-4.png");
        deletePaper.color = Color.white;
        deletePaper.preserveAspect = true;
        deletePaper.raycastTarget = false;

        Stretch(deleteButton.GetComponent<RectTransform>());
        Image deleteHit = EnsureImage(deleteButton.gameObject);
        deleteHit.sprite = null;
        deleteHit.color = new Color(1f, 1f, 1f, 0.001f);
        deleteHit.raycastTarget = true;
        deleteButton.targetGraphic = deleteHit;
        SetButtonLabel(deleteButton, "DELETE FILE\nSpace", 40f, Vector2.zero, new Vector2(520f, 130f));
        EditorUtility.SetDirty(deleteRoot);
        EditorUtility.SetDirty(deleteButton);
    }

    private static void ConfigurePaperButton(
        Button button,
        string spritePath,
        Vector2 position,
        Vector2 size,
        string label)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        SetCentered(rect, position, size);
        Image image = EnsureImage(button.gameObject);
        image.sprite = LoadSprite(spritePath);
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = true;
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        SetButtonLabel(button, label, 38f, new Vector2(0f, -45f), new Vector2(310f, 130f));
        EditorUtility.SetDirty(button);
        EditorUtility.SetDirty(rect);
    }

    private static void SetButtonLabel(Button button, string text, float fontSize, Vector2 position, Vector2 size)
    {
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label == null)
        {
            return;
        }

        label.gameObject.SetActive(true);
        label.text = text;
        label.fontSize = fontSize;
        label.fontStyle = FontStyles.Bold;
        label.color = new Color32(73, 42, 35, 255);
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        SetCentered(label.rectTransform, position, size);
        EditorUtility.SetDirty(label);
    }

    private static void ConfigureNavigationScreen(
        MainMenuController controller,
        MainMenuFlowView flowView,
        GameObject savePanel)
    {
        UIScreen screen = savePanel.GetComponent<UIScreen>();
        if (screen == null)
        {
            screen = savePanel.AddComponent<UIScreen>();
        }

        screen.Configure(UIRoute.SaveSelect, savePanel, flowView.FirstSaveSlotButton);
        var serializedController = new SerializedObject(controller);
        serializedController.FindProperty("saveSelectScreen").objectReferenceValue = screen;
        serializedController.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(screen);
    }

    private static void ConfigureImage(Transform target, string spritePath, Vector2 position, Vector2 size)
    {
        SetCentered(target as RectTransform, position, size);
        Image image = EnsureImage(target.gameObject);
        image.sprite = LoadSprite(spritePath);
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = false;
        EditorUtility.SetDirty(image);
    }

    private static Image EnsureImage(GameObject target)
    {
        Image image = target.GetComponent<Image>();
        return image != null ? image : target.AddComponent<Image>();
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

    private static Transform RequireChild(Transform parent, string objectName)
    {
        Transform child = FindChild(parent, objectName);
        if (child == null)
        {
            throw new InvalidOperationException($"'{objectName}' was not found under '{parent.name}'.");
        }

        return child;
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

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            throw new FileNotFoundException($"Required Save Select sprite is missing or not imported as Sprite: {path}");
        }

        return sprite;
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

    private static void SetCentered(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
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
}
