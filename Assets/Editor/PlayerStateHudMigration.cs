using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
internal static class PlayerStateHudMigration
{
    private const string PrefabPath = "Assets/Prefebs/PlayerState.prefab";
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string SessionKey = "Potato.PlayerStateHudMigration.20260826";

    static PlayerStateHudMigration()
    {
        EditorApplication.delayCall += RunOnce;
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
    }

    private static void HandlePlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.delayCall += RunOnce;
        }
    }

    private static void RunOnce()
    {
        if (SessionState.GetBool(SessionKey, false) || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        try
        {
            EnsurePrefab();
            EnsureScene();
            AssetDatabase.SaveAssets();
            SessionState.SetBool(SessionKey, true);
            Debug.Log("PlayerState HUD migration finished: Experience and StoredMaterials are serialized under PlayerState.");
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private static void EnsurePrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
        try
        {
            if (EnsureHud(root.transform))
            {
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void EnsureScene()
    {
        Scene scene = SceneManager.GetSceneByPath(ScenePath);
        bool openedForMigration = !scene.IsValid() || !scene.isLoaded;
        if (openedForMigration)
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
        }

        GameObject playerState = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
            .FirstOrDefault(candidate => candidate.name == "PlayerState")
            ?.gameObject;
        if (playerState != null && EnsureHud(playerState.transform))
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        if (openedForMigration)
        {
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static bool EnsureHud(Transform playerState)
    {
        bool changed = false;
        changed |= EnsureExperience(playerState);
        changed |= EnsureStoredMaterials(playerState);

        if (playerState.GetComponent<PlayerExperienceHudView>() == null)
        {
            playerState.gameObject.AddComponent<PlayerExperienceHudView>();
            changed = true;
        }

        return changed;
    }

    private static bool EnsureExperience(Transform playerState)
    {
        if (playerState.Find("Experience") != null)
        {
            return false;
        }

        RectTransform blood = playerState.Find("Blood") as RectTransform;
        if (blood == null)
        {
            return false;
        }

        GameObject experience = Object.Instantiate(blood.gameObject, playerState);
        experience.name = "Experience";
        RectTransform experienceRect = experience.GetComponent<RectTransform>();
        experienceRect.anchoredPosition = blood.anchoredPosition
            + Vector2.down * (blood.sizeDelta.y + 125f);
        experienceRect.sizeDelta = new Vector2(blood.sizeDelta.x, 64f);

        Image background = experience.GetComponent<Image>();
        if (background != null)
        {
            background.raycastTarget = false;
        }

        Image fill = FindDirectChild(experience.transform, "Fill")?.GetComponent<Image>();
        if (fill != null)
        {
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 0f;
            fill.color = new Color(0.22f, 0.88f, 0.32f, 1f);
            fill.raycastTarget = false;
            Stretch(fill.rectTransform, Vector2.zero, Vector2.one);
        }

        TMP_Text[] texts = experience.GetComponentsInChildren<TMP_Text>(true);
        TMP_Text level = texts.FirstOrDefault(text => text.name == "CurrentHealth") ?? texts.FirstOrDefault();
        TMP_Text value = texts.FirstOrDefault(text => text.name == "MaxHealth") ?? texts.LastOrDefault();
        foreach (TMP_Text text in texts)
        {
            if (text != level && text != value)
            {
                Object.DestroyImmediate(text.gameObject);
            }
        }

        if (level != null)
        {
            level.name = "Level";
            level.text = "Lv.1";
            level.fontSize = 28f;
            level.enableAutoSizing = true;
            level.fontSizeMin = 18f;
            level.fontSizeMax = 32f;
            level.alignment = TextAlignmentOptions.Center;
            level.raycastTarget = false;
            Stretch(level.rectTransform, new Vector2(0.02f, 0.08f), new Vector2(0.24f, 0.92f));
        }

        if (value != null)
        {
            value.name = "Value";
            value.text = "0 / 10";
            value.fontSize = 24f;
            value.enableAutoSizing = true;
            value.fontSizeMin = 16f;
            value.fontSizeMax = 28f;
            value.alignment = TextAlignmentOptions.Center;
            value.raycastTarget = false;
            Stretch(value.rectTransform, new Vector2(0.24f, 0.08f), new Vector2(0.98f, 0.92f));
        }

        return true;
    }

    private static bool EnsureStoredMaterials(Transform playerState)
    {
        if (playerState.Find("StoredMaterials") != null)
        {
            return false;
        }

        RectTransform coin = playerState.Find("Coin") as RectTransform;
        if (coin == null)
        {
            return false;
        }

        GameObject storedMaterials = Object.Instantiate(coin.gameObject, playerState);
        storedMaterials.name = "StoredMaterials";
        RectTransform storedRect = storedMaterials.GetComponent<RectTransform>();
        TMP_Text storedText = storedMaterials.GetComponentInChildren<TMP_Text>(true);
        float textWidth = storedText != null ? storedText.rectTransform.sizeDelta.x : 240f;
        storedRect.anchoredPosition = coin.anchoredPosition
            + Vector2.right * (coin.sizeDelta.x + textWidth + 30f);

        if (storedText != null)
        {
            storedText.name = "StoredMaterialNum";
            storedText.text = "储存 0";
            storedText.enableAutoSizing = true;
            storedText.fontSizeMin = 20f;
            storedText.fontSizeMax = 48f;
            storedText.raycastTarget = false;
        }

        return true;
    }

    private static Transform FindDirectChild(Transform parent, string childName)
    {
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    private static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
