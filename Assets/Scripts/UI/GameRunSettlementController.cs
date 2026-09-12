using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameRunSettlementController : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";

    [Header("Scene UI References")]
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text outcomeText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    private EnemySpawner spawner;
    private PlayerHealth playerHealth;
    private float runStartedAt;
    private float restoredElapsedSeconds;
    private bool hasBoundRun;
    private bool settlementStarted;
    private bool resultVisible;

    public float ElapsedRunSeconds => restoredElapsedSeconds
        + (hasBoundRun ? Mathf.Max(0f, Time.realtimeSinceStartup - runStartedAt) : 0f);

    private void Awake()
    {
        AutoBindReferences();
        BindSceneButtons();
        if (!HasSceneReferences())
        {
            Debug.LogError("Settlement UI is missing from SampleScene. Rebuild it with Tools/Potato UI/Build Settlement Panel In SampleScene.", this);
        }
        SetVisible(false);
    }

    private void OnDestroy()
    {
        UnbindPlayerHealth();
        UnbindSceneButtons();
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

        Debug.LogError("GameRunSettlementController must be placed in the gameplay scene.");
        return null;
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
        if (settlementStarted || !HasSceneReferences())
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

    [ContextMenu("Auto Bind Scene References")]
    public void AutoBindReferences()
    {
        windowRoot = FindDescendant("SettlementWindow")?.gameObject;
        canvasGroup = windowRoot != null ? windowRoot.GetComponent<CanvasGroup>() : null;
        titleText = FindComponent<TMP_Text>("Title");
        outcomeText = FindComponent<TMP_Text>("Outcome");
        statsText = FindComponent<TMP_Text>("Stats");
        restartButton = FindComponent<Button>("RestartButton");
        mainMenuButton = FindComponent<Button>("MainMenuButton");
    }

    private static string FormatDuration(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(seconds));
        return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }

    private bool HasSceneReferences()
    {
        return windowRoot != null
            && canvasGroup != null
            && titleText != null
            && outcomeText != null
            && statsText != null
            && restartButton != null
            && mainMenuButton != null;
    }

    private void BindSceneButtons()
    {
        UnbindSceneButtons();
        restartButton?.onClick.AddListener(RestartRun);
        mainMenuButton?.onClick.AddListener(ReturnToMainMenu);
    }

    private void UnbindSceneButtons()
    {
        restartButton?.onClick.RemoveListener(RestartRun);
        mainMenuButton?.onClick.RemoveListener(ReturnToMainMenu);
    }

    private Transform FindDescendant(string objectName)
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

    private T FindComponent<T>(string objectName) where T : Component
    {
        Transform target = FindDescendant(objectName);
        return target != null ? target.GetComponent<T>() : null;
    }

}
