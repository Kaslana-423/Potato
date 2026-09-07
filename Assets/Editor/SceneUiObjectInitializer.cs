using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SceneUiObjectInitializer
{
    private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
    private const string GameplayScenePath = "Assets/Scenes/SampleScene.unity";
    private const string ChineseFontPath =
        "Assets/TextMesh Pro/Resources/Fonts & Materials/SmileySans-Oblique SDF.asset";

    [MenuItem("Tools/Potato UI/Initialize Scene UI Objects")]
    public static void InitializeMissingSceneUiObjects()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.delayCall += InitializeMissingSceneUiObjects;
            return;
        }

        InitializeScene(MainMenuScenePath, InitializeMainMenu);
        InitializeScene(GameplayScenePath, InitializeGameplayPause);
    }

    private static void InitializeScene(string scenePath, System.Func<Scene, bool> initializer)
    {
        Scene scene = SceneManager.GetSceneByPath(scenePath);
        bool openedForInitialization = !scene.IsValid() || !scene.isLoaded;
        if (openedForInitialization)
        {
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }
        else if (scene.isDirty)
        {
            Debug.LogWarning($"Skipped UI object initialization for '{scenePath}' because it has unsaved changes.");
            return;
        }

        List<RectTransformSnapshot> snapshots = CaptureRectTransforms(scene);
        try
        {
            if (!initializer(scene))
            {
                return;
            }

            RestoreRectTransforms(snapshots);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Initialized scene UI objects in '{scenePath}'.");
        }
        finally
        {
            if (openedForInitialization && scene.IsValid() && scene.isLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }

    private static bool InitializeMainMenu(Scene scene)
    {
        MainMenuController controller = FindSceneComponent<MainMenuController>(scene);
        if (controller == null)
        {
            Debug.LogError("MainMenu.unity does not contain a MainMenuController.");
            return false;
        }

        controller.AutoBindReferences();
        GameObject mainActionsPanel = FindChild(controller.transform, "MainActionsPanel")?.gameObject;
        GameObject settingsPanel = FindChild(controller.transform, "SettingsPanel")?.gameObject;
        if (mainActionsPanel == null || settingsPanel == null)
        {
            Debug.LogError("MainMenu.unity is missing MainActionsPanel or SettingsPanel.");
            return false;
        }

        MainMenuFlowView navigationView = controller.GetComponent<MainMenuFlowView>();
        bool pagesExist = navigationView != null
            && navigationView.TitlePanel != null
            && navigationView.SaveSelectPanel != null
            && navigationView.CharacterSelectPanel != null;
        UIRouter router = controller.GetComponent<UIRouter>();
        bool screensExist = pagesExist
            && router != null
            && navigationView.TitlePanel.GetComponent<UIScreen>() != null
            && navigationView.SaveSelectPanel.GetComponent<UIScreen>() != null
            && navigationView.CharacterSelectPanel.GetComponent<UIScreen>() != null
            && mainActionsPanel.GetComponent<UIScreen>() != null
            && settingsPanel.GetComponent<UIScreen>() != null;
        if (controller.HasSceneNavigationReferences && screensExist)
        {
            return false;
        }

        if (navigationView == null)
        {
            navigationView = controller.gameObject.AddComponent<MainMenuFlowView>();
        }

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ChineseFontPath);
        navigationView.EnsurePages(mainActionsPanel, font);
        router = router != null ? router : controller.gameObject.AddComponent<UIRouter>();

        UIScreen titleScreen = ConfigureScreen(
            navigationView.TitlePanel,
            UIRoute.Title,
            navigationView.TitleContinueButton);
        UIScreen saveSelectScreen = ConfigureScreen(
            navigationView.SaveSelectPanel,
            UIRoute.SaveSelect,
            navigationView.FirstSaveSlotButton);
        UIScreen mainMenuScreen = ConfigureScreen(mainActionsPanel, UIRoute.MainMenu, FindButton(mainActionsPanel, "StartGameButton"));
        UIScreen characterSelectScreen = ConfigureScreen(
            navigationView.CharacterSelectPanel,
            UIRoute.CharacterSelect,
            navigationView.CharacterStartButton);
        UIScreen settingsScreen = ConfigureScreen(settingsPanel, UIRoute.Settings, FindSlider(settingsPanel, "VolumeSlider"));

        controller.ConfigureNavigationReferences(
            mainActionsPanel,
            navigationView,
            router,
            titleScreen,
            saveSelectScreen,
            mainMenuScreen,
            characterSelectScreen,
            settingsScreen);
        EditorUtility.SetDirty(navigationView);
        EditorUtility.SetDirty(router);
        EditorUtility.SetDirty(controller);
        return true;
    }

    private static bool InitializeGameplayPause(Scene scene)
    {
        GameplayPauseController controller = FindSceneComponent<GameplayPauseController>(scene);
        if (controller != null && controller.HasSceneUiReferences)
        {
            return false;
        }

        if (controller == null)
        {
            GameObject controllerObject = new GameObject("GameplayPauseController");
            SceneManager.MoveGameObjectToScene(controllerObject, scene);
            controller = controllerObject.AddComponent<GameplayPauseController>();
        }

        controller.BuildSceneUi();
        EditorUtility.SetDirty(controller);
        return controller.HasSceneUiReferences;
    }

    private static UIScreen ConfigureScreen(GameObject root, UIRoute route, Selectable initialSelection)
    {
        UIScreen screen = root.GetComponent<UIScreen>();
        if (screen == null)
        {
            screen = root.AddComponent<UIScreen>();
        }

        screen.Configure(route, root, initialSelection);
        EditorUtility.SetDirty(screen);
        return screen;
    }

    private static Button FindButton(GameObject root, string objectName)
    {
        Transform child = FindChild(root.transform, objectName);
        return child != null ? child.GetComponent<Button>() : null;
    }

    private static Slider FindSlider(GameObject root, string objectName)
    {
        Transform child = FindChild(root.transform, objectName);
        return child != null ? child.GetComponent<Slider>() : null;
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

    private static List<RectTransformSnapshot> CaptureRectTransforms(Scene scene)
    {
        var snapshots = new List<RectTransformSnapshot>();
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            foreach (RectTransform rect in rootObject.GetComponentsInChildren<RectTransform>(true))
            {
                snapshots.Add(new RectTransformSnapshot(rect));
            }
        }

        return snapshots;
    }

    private static void RestoreRectTransforms(List<RectTransformSnapshot> snapshots)
    {
        foreach (RectTransformSnapshot snapshot in snapshots)
        {
            snapshot.Restore();
        }
    }

    private readonly struct RectTransformSnapshot
    {
        private readonly RectTransform rect;
        private readonly Vector2 anchorMin;
        private readonly Vector2 anchorMax;
        private readonly Vector2 pivot;
        private readonly Vector3 anchoredPosition;
        private readonly Vector2 sizeDelta;
        private readonly Quaternion localRotation;
        private readonly Vector3 localScale;

        public RectTransformSnapshot(RectTransform rect)
        {
            this.rect = rect;
            anchorMin = rect.anchorMin;
            anchorMax = rect.anchorMax;
            pivot = rect.pivot;
            anchoredPosition = rect.anchoredPosition3D;
            sizeDelta = rect.sizeDelta;
            localRotation = rect.localRotation;
            localScale = rect.localScale;
        }

        public void Restore()
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition3D = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            rect.localRotation = localRotation;
            rect.localScale = localScale;
        }
    }
}
