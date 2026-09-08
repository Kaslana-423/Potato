using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GameplayCanvasSceneMigration
{
    private const string GameplayScenePath = "Assets/Scenes/SampleScene.unity";
    private static readonly Vector2 ReferenceResolution = new Vector2(2560f, 1440f);

    [InitializeOnLoadMethod]
    private static void ScheduleMigration()
    {
        EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        EditorApplication.delayCall += UpgradeGameplayCanvas;
    }

    [MenuItem("Tools/Potato UI/Upgrade Gameplay Canvas")]
    public static void UpgradeGameplayCanvas()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += UpgradeGameplayCanvas;
            return;
        }

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        Scene scene = SceneManager.GetSceneByPath(GameplayScenePath);
        bool openedForMigration = !scene.IsValid() || !scene.isLoaded;
        if (openedForMigration)
        {
            scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Additive);
        }
        else if (scene.isDirty)
        {
            Debug.LogWarning("Skipped SampleScene Canvas migration because the scene has unsaved changes.");
            return;
        }

        try
        {
            bool changed = UpgradeCanvasHierarchy(scene);
            changed |= EnsureExperienceBinding(scene);
            if (!changed)
            {
                return;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("SampleScene Canvas upgraded to a responsive, scene-bound gameplay UI.");
        }
        finally
        {
            if (openedForMigration && scene.IsValid() && scene.isLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }

    private static bool UpgradeCanvasHierarchy(Scene scene)
    {
        Canvas gameplayCanvas = FindGameplayCanvas(scene);
        if (gameplayCanvas == null)
        {
            Debug.LogError("SampleScene does not contain the Canvas that owns PlayerState.");
            return false;
        }

        bool changed = false;
        Transform oldShopRoot = gameplayCanvas.transform.parent;
        if (oldShopRoot != null
            && oldShopRoot.name == "ShopRoot"
            && oldShopRoot.childCount == 1
            && oldShopRoot.GetComponents<Component>().Length == 1)
        {
            gameplayCanvas.transform.SetParent(oldShopRoot.parent, false);
            Object.DestroyImmediate(oldShopRoot.gameObject);
            changed = true;
        }

        if (gameplayCanvas.name != "GameplayCanvas")
        {
            gameplayCanvas.name = "GameplayCanvas";
            changed = true;
        }

        CanvasScaler scaler = gameplayCanvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameplayCanvas.gameObject.AddComponent<CanvasScaler>();
            changed = true;
        }

        if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize
            || scaler.referenceResolution != ReferenceResolution
            || scaler.screenMatchMode != CanvasScaler.ScreenMatchMode.MatchWidthOrHeight
            || !Mathf.Approximately(scaler.matchWidthOrHeight, 0.5f))
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            EditorUtility.SetDirty(scaler);
            changed = true;
        }

        Transform playerState = gameplayCanvas.transform.Find("PlayerState");
        if (playerState != null)
        {
            changed |= AnchorToTopLeft(playerState.Find("Blood") as RectTransform);
            changed |= AnchorToTopLeft(playerState.Find("Experience") as RectTransform);
            changed |= AnchorToTopLeft(playerState.Find("Coin") as RectTransform);
            changed |= AnchorToTopLeft(playerState.Find("StoredMaterials") as RectTransform);
        }

        if (changed)
        {
            EditorUtility.SetDirty(gameplayCanvas.gameObject);
        }

        return changed;
    }

    private static bool EnsureExperienceBinding(Scene scene)
    {
        PlayerExperienceHudView hudView = FindSceneComponent<PlayerExperienceHudView>(scene);
        if (hudView == null)
        {
            Debug.LogError("SampleScene PlayerState is missing PlayerExperienceHudView.");
            return false;
        }

        bool changed = false;
        PlayerExperience experience = FindSceneComponent<PlayerExperience>(scene);
        if (experience == null)
        {
            PlayerStats playerStats = FindSceneComponent<PlayerStats>(scene);
            if (playerStats == null)
            {
                Debug.LogError("SampleScene is missing PlayerStats, so PlayerExperience could not be initialized.");
                return false;
            }

            experience = playerStats.gameObject.AddComponent<PlayerExperience>();
            EditorUtility.SetDirty(experience);
            changed = true;
        }

        if (!hudView.HasSceneReferences)
        {
            hudView.AutoBindReferences();
            EditorUtility.SetDirty(hudView);
            changed = true;
        }

        return changed;
    }

    private static Canvas FindGameplayCanvas(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Canvas canvas in root.GetComponentsInChildren<Canvas>(true))
            {
                if (canvas.GetComponentInChildren<PlayerHealthBarView>(true) != null)
                {
                    return canvas;
                }
            }
        }

        return null;
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T component = root.GetComponentInChildren<T>(true);
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }

    private static bool AnchorToTopLeft(RectTransform rect)
    {
        if (rect == null
            || rect.anchorMin != rect.anchorMax
            || rect.anchorMin == new Vector2(0f, 1f))
        {
            return false;
        }

        Vector2 visualPosition = Vector2.Scale(rect.anchorMin, ReferenceResolution) + rect.anchoredPosition;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.anchoredPosition = visualPosition - new Vector2(0f, ReferenceResolution.y);
        EditorUtility.SetDirty(rect);
        return true;
    }

    private static void HandlePlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.delayCall += UpgradeGameplayCanvas;
        }
    }
}
