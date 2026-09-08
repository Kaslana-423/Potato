using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ResolutionSettingsSceneAssembler
{
    private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
    private const string GameplayScenePath = "Assets/Scenes/SampleScene.unity";
    private const string MarkerPath = "Assets/Editor/RunResolutionSettingsAssembly.once";

    private static double nextRetryTime;

    static ResolutionSettingsSceneAssembler()
    {
        EditorApplication.update -= TryRunRequestedAssembly;
        EditorApplication.update += TryRunRequestedAssembly;
    }

    [MenuItem("Tools/Potato UI/Assemble Resolution Settings")]
    public static void AssembleFromMenu()
    {
        AssembleAndSave(MainMenuScenePath, "SettingsWindow", true);
        AssembleAndSave(GameplayScenePath, "PauseSettingsPanel", false);
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
        if (HasUnsavedLoadedScene(MainMenuScenePath) || HasUnsavedLoadedScene(GameplayScenePath))
        {
            Debug.Log("Resolution settings assembly is waiting for the open scenes to be saved.");
            return;
        }

        try
        {
            AssembleFromMenu();
            DeleteMarker();
            AssetDatabase.Refresh();
            EditorApplication.update -= TryRunRequestedAssembly;
            Debug.Log("Resolution dropdowns were assembled in MainMenu and SampleScene.");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private static void AssembleAndSave(string scenePath, string settingsRootName, bool isMainMenu)
    {
        Scene scene = SceneManager.GetSceneByPath(scenePath);
        bool openedForAssembly = !scene.IsValid() || !scene.isLoaded;
        if (openedForAssembly)
        {
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }

        try
        {
            Transform settingsRoot = FindChild(scene, settingsRootName);
            if (settingsRoot == null)
            {
                throw new InvalidOperationException($"{settingsRootName} was not found in {scenePath}.");
            }

            Transform oldFullscreenToggle = FindChild(settingsRoot, "FullscreenToggle");
            if (oldFullscreenToggle != null)
            {
                oldFullscreenToggle.gameObject.SetActive(false);
                EditorUtility.SetDirty(oldFullscreenToggle.gameObject);
            }

            TMP_Text styleSource = FindStyleSource(settingsRoot, isMainMenu);
            TMP_Text resolutionLabel = EnsureLabel(settingsRoot, styleSource, isMainMenu);
            TMP_Dropdown dropdown = EnsureDropdown(settingsRoot, resolutionLabel.font, isMainMenu);
            dropdown.transform.SetAsLastSibling();

            EditorUtility.SetDirty(resolutionLabel);
            EditorUtility.SetDirty(dropdown);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
            {
                throw new IOException($"{scenePath} could not be saved after resolution UI assembly.");
            }
        }
        finally
        {
            if (openedForAssembly && scene.IsValid() && scene.isLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }

    private static TMP_Text EnsureLabel(Transform parent, TMP_Text styleSource, bool isMainMenu)
    {
        string objectName = isMainMenu ? "ResolutionLabelText" : "ResolutionLabel";
        Transform existing = FindChild(parent, objectName);
        TMP_Text label;
        if (existing != null)
        {
            label = existing.GetComponent<TMP_Text>();
        }
        else
        {
            GameObject labelObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObject.layer = parent.gameObject.layer;
            labelObject.transform.SetParent(parent, false);
            label = labelObject.GetComponent<TMP_Text>();
        }

        if (label == null)
        {
            throw new InvalidOperationException($"{objectName} exists but has no TMP_Text component.");
        }

        if (styleSource != null)
        {
            label.font = styleSource.font;
            label.fontSize = styleSource.fontSize;
            label.fontStyle = styleSource.fontStyle;
            label.color = styleSource.color;
        }

        label.text = "分辨率";
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.raycastTarget = false;
        SetNormalizedRect(
            label.rectTransform,
            isMainMenu ? new Vector2(0.08f, 0.27f) : new Vector2(0.12f, 0.43f),
            isMainMenu ? new Vector2(0.37f, 0.41f) : new Vector2(0.42f, 0.55f));
        return label;
    }

    private static TMP_Dropdown EnsureDropdown(Transform parent, TMP_FontAsset font, bool isMainMenu)
    {
        Transform existing = FindChild(parent, "ResolutionDropdown");
        TMP_Dropdown dropdown = existing != null ? existing.GetComponent<TMP_Dropdown>() : null;
        if (dropdown == null)
        {
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing.gameObject);
            }

            dropdown = ResolutionDropdownSetting.CreateSceneDropdown(parent, font);
        }
        else if (dropdown.GetComponent<ResolutionDropdownSetting>() == null)
        {
            dropdown.gameObject.AddComponent<ResolutionDropdownSetting>();
        }

        SetLayerRecursively(dropdown.gameObject, parent.gameObject.layer);
        SetNormalizedRect(
            dropdown.GetComponent<RectTransform>(),
            isMainMenu ? new Vector2(0.38f, 0.285f) : new Vector2(0.43f, 0.445f),
            isMainMenu ? new Vector2(0.92f, 0.395f) : new Vector2(0.88f, 0.535f));
        ConfigureTemplate(dropdown);
        return dropdown;
    }

    private static void ConfigureTemplate(TMP_Dropdown dropdown)
    {
        RectTransform template = dropdown.template;
        if (template == null)
        {
            throw new InvalidOperationException("ResolutionDropdown has no dropdown template.");
        }

        template.anchorMin = new Vector2(0f, 1f);
        template.anchorMax = Vector2.one;
        template.pivot = new Vector2(0.5f, 0f);
        template.anchoredPosition = new Vector2(0f, -2f);
        template.sizeDelta = new Vector2(0f, 176f);

        ScrollRect scrollRect = template.GetComponent<ScrollRect>();
        if (scrollRect != null && scrollRect.content != null)
        {
            scrollRect.content.sizeDelta = new Vector2(0f, 48f);
            Toggle item = scrollRect.content.GetComponentInChildren<Toggle>(true);
            if (item != null)
            {
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 42f);
            }
        }
    }

    private static TMP_Text FindStyleSource(Transform settingsRoot, bool isMainMenu)
    {
        Transform preferred = FindChild(settingsRoot, isMainMenu ? "VolumeLabelText" : "VolumeLabel");
        TMP_Text preferredText = preferred != null ? preferred.GetComponent<TMP_Text>() : null;
        return preferredText != null ? preferredText : settingsRoot.GetComponentInChildren<TMP_Text>(true);
    }

    private static void SetNormalizedRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static void SetLayerRecursively(GameObject root, int layer)
    {
        root.layer = layer;
        foreach (Transform child in root.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private static Transform FindChild(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform result = FindChild(root.transform, objectName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static Transform FindChild(Transform root, string objectName)
    {
        if (root.name == objectName)
        {
            return root;
        }

        foreach (Transform child in root)
        {
            Transform result = FindChild(child, objectName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static bool HasUnsavedLoadedScene(string scenePath)
    {
        Scene scene = SceneManager.GetSceneByPath(scenePath);
        return scene.IsValid() && scene.isLoaded && scene.isDirty;
    }

    private static void DeleteMarker()
    {
        string markerPath = Path.GetFullPath(MarkerPath);
        if (File.Exists(markerPath))
        {
            File.Delete(markerPath);
        }

        string markerMetaPath = markerPath + ".meta";
        if (File.Exists(markerMetaPath))
        {
            File.Delete(markerMetaPath);
        }
    }
}
