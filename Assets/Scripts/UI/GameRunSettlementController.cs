using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameRunSettlementController : MonoBehaviour
{
    private const string FontResourcePath = "Fonts & Materials/SmileySans-Oblique SDF";
    private const string MainMenuSceneName = "MainMenu";

    private EnemySpawner spawner;
    private PlayerHealth playerHealth;
    private GameObject windowRoot;
    private CanvasGroup canvasGroup;
    private TMP_Text titleText;
    private TMP_Text outcomeText;
    private TMP_Text statsText;
    private float runStartedAt;
    private float restoredElapsedSeconds;
    private bool hasBoundRun;
    private bool settlementStarted;
    private bool resultVisible;

    public float ElapsedRunSeconds => restoredElapsedSeconds
        + (hasBoundRun ? Mathf.Max(0f, Time.realtimeSinceStartup - runStartedAt) : 0f);

    private void Awake()
    {
        BuildUi();
        SetVisible(false);
    }

    private void OnDestroy()
    {
        UnbindPlayerHealth();
        if (resultVisible)
        {
            Time.timeScale = 1f;
        }
    }

    public static GameRunSettlementController GetOrCreate()
    {
        GameRunSettlementController existing = FindObjectOfType<GameRunSettlementController>(true);
        if (existing != null)
        {
            return existing;
        }

        GameObject controllerObject = new GameObject("GameRunSettlementController");
        return controllerObject.AddComponent<GameRunSettlementController>();
    }

    public void Bind(EnemySpawner enemySpawner, PlayerHealth health)
    {
        spawner = enemySpawner;
        if (!hasBoundRun)
        {
            runStartedAt = Time.realtimeSinceStartup;
            hasBoundRun = true;
        }

        if (health == null)
        {
            health = FindObjectOfType<PlayerHealth>();
        }

        if (playerHealth == health)
        {
            return;
        }

        UnbindPlayerHealth();
        playerHealth = health;
        if (playerHealth != null)
        {
            playerHealth.Died += HandlePlayerDied;
        }
    }

    public void ShowVictory()
    {
        BeginSettlement(true);
    }

    public void RestoreElapsedRunSeconds(float elapsedSeconds)
    {
        restoredElapsedSeconds = Mathf.Max(0f, elapsedSeconds);
        runStartedAt = Time.realtimeSinceStartup;
        hasBoundRun = true;
    }

    private void HandlePlayerDied(PlayerHealth health)
    {
        BeginSettlement(false);
    }

    private void BeginSettlement(bool victory)
    {
        if (settlementStarted)
        {
            return;
        }

        settlementStarted = true;
        UnbindPlayerHealth();
        spawner?.EndRunCombat();
        GameSessionState.AbandonRun();
        PopulateResult(victory);

        if (!victory && playerHealth != null)
        {
            PlayerDeathEffect deathEffect = playerHealth.GetComponent<PlayerDeathEffect>();
            if (deathEffect == null)
            {
                deathEffect = playerHealth.gameObject.AddComponent<PlayerDeathEffect>();
            }

            deathEffect.Play(ShowResult);
            return;
        }

        ShowResult();
    }

    private void PopulateResult(bool victory)
    {
        int wave = spawner != null ? spawner.CurrentWave : 1;
        int kills = spawner != null ? spawner.TotalEnemiesKilled : 0;
        int level = PlayerStats.Instance != null ? PlayerStats.Instance.Level : 1;
        PlayerWallet wallet = PlayerWallet.GetOrCreate();
        int materials = wallet != null ? wallet.Coins : 0;
        int retainedMaterials = wallet != null ? wallet.RetainedMaterials : 0;
        float elapsedSeconds = ElapsedRunSeconds;

        titleText.text = victory ? "胜利" : "本局结束";
        titleText.color = victory
            ? new Color(1f, 0.82f, 0.28f, 1f)
            : new Color(1f, 0.32f, 0.32f, 1f);
        outcomeText.text = victory
            ? $"你完成了第 {wave} 波"
            : $"你倒在了第 {wave} 波";
        statsText.text =
            $"角色等级    Lv.{level}\n" +
            $"击杀敌人    {kills}\n" +
            $"持有材料    {materials}\n" +
            $"保留材料    {retainedMaterials}\n" +
            $"游戏时间    {FormatDuration(elapsedSeconds)}";
    }

    private void ShowResult()
    {
        resultVisible = true;
        SetVisible(true);
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        canvasGroup.alpha = 0f;
        const float fadeDuration = 0.22f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        Time.timeScale = 0f;
    }

    private void RestartRun()
    {
        Time.timeScale = 1f;
        GameSessionState.BeginNewRun();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GameSessionState.AbandonRun();
        if (!Application.CanStreamedLevelBeLoaded(MainMenuSceneName))
        {
            Debug.LogError($"Main menu scene is not in Build Settings: {MainMenuSceneName}", this);
            return;
        }

        SceneManager.LoadScene(MainMenuSceneName);
    }

    private void UnbindPlayerHealth()
    {
        if (playerHealth != null)
        {
            playerHealth.Died -= HandlePlayerDied;
        }
    }

    private void SetVisible(bool visible)
    {
        if (windowRoot != null)
        {
            windowRoot.SetActive(visible);
        }
    }

    private void BuildUi()
    {
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>(FontResourcePath);
        GameObject canvasObject = new GameObject(
            "GameSettlementCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        canvasObject.layer = 5;
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        ResponsiveUiLayout.ConfigureCanvasScaler(scaler);

        windowRoot = CreateUiObject("SettlementWindow", canvasObject.transform);
        Stretch(windowRoot.GetComponent<RectTransform>());
        canvasGroup = windowRoot.AddComponent<CanvasGroup>();
        Image dimmer = windowRoot.AddComponent<Image>();
        dimmer.color = new Color(0.01f, 0.012f, 0.018f, 0.94f);

        GameObject panel = CreateUiObject("Panel", windowRoot.transform);
        ResponsiveUiLayout.SetNormalizedRect(
            panel.GetComponent<RectTransform>(),
            new Vector2(0.3021f, 0.1481f),
            new Vector2(0.6979f, 0.8519f));
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.065f, 0.07f, 0.085f, 1f);

        titleText = CreateText("Title", panel.transform, font, 62f, FontStyles.Bold);
        SetRect(titleText.rectTransform, new Vector2(0.07f, 0.81f), new Vector2(0.93f, 0.96f));
        titleText.alignment = TextAlignmentOptions.Center;

        outcomeText = CreateText("Outcome", panel.transform, font, 28f, FontStyles.Bold);
        SetRect(outcomeText.rectTransform, new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.82f));
        outcomeText.alignment = TextAlignmentOptions.Center;
        outcomeText.color = new Color(0.88f, 0.9f, 0.94f, 1f);

        statsText = CreateText("Stats", panel.transform, font, 28f, FontStyles.Normal);
        SetRect(statsText.rectTransform, new Vector2(0.2f, 0.28f), new Vector2(0.8f, 0.68f));
        statsText.alignment = TextAlignmentOptions.MidlineLeft;
        statsText.lineSpacing = 18f;
        statsText.color = new Color(0.93f, 0.94f, 0.97f, 1f);

        CreateButton(
            "RestartButton",
            panel.transform,
            font,
            "重新开始",
            new Vector2(0.1f, 0.08f),
            new Vector2(0.47f, 0.21f),
            new Color(0.22f, 0.58f, 0.3f, 1f),
            RestartRun);
        CreateButton(
            "MainMenuButton",
            panel.transform,
            font,
            "返回主菜单",
            new Vector2(0.53f, 0.08f),
            new Vector2(0.9f, 0.21f),
            new Color(0.3f, 0.34f, 0.42f, 1f),
            ReturnToMainMenu);
    }

    private static string FormatDuration(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(seconds));
        return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }

    private static Button CreateButton(
        string objectName,
        Transform parent,
        TMP_FontAsset font,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Color color,
        UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        SetRect(buttonObject.GetComponent<RectTransform>(), anchorMin, anchorMax);
        Image image = buttonObject.AddComponent<Image>();
        image.color = color;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        TMP_Text labelText = CreateText("Text", buttonObject.transform, font, 27f, FontStyles.Bold);
        Stretch(labelText.rectTransform);
        labelText.text = label;
        labelText.alignment = TextAlignmentOptions.Center;
        return button;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject result = new GameObject(objectName, typeof(RectTransform));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static TMP_Text CreateText(
        string objectName,
        Transform parent,
        TMP_FontAsset font,
        float fontSize,
        FontStyles style)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        if (font != null)
        {
            text.font = font;
        }

        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Stretch(RectTransform rect)
    {
        SetRect(rect, Vector2.zero, Vector2.one);
    }

}
