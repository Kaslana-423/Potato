using TMPro;
using UnityEngine;

public sealed class EnemyWaveHudView : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private bool bindSpawnerOnEnable = true;
    [SerializeField] private bool autoFindTexts = true;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private bool useMinuteSecondCounter = false;

    private int lastDisplayedWave = int.MinValue;
    private int lastDisplayedCounter = int.MinValue;

    private void Awake()
    {
        AutoBindReferences();
    }

    private void OnEnable()
    {
        AutoBindReferences();
        Refresh(true);
    }

    private void Update()
    {
        Refresh(false);
    }

    private void Reset()
    {
        AutoBindReferences();
    }

    private void OnValidate()
    {
        AutoBindReferences();
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        if (bindSpawnerOnEnable && enemySpawner == null && Application.isPlaying)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>(true);
        }

        if (!autoFindTexts)
        {
            return;
        }

        if (waveText == null)
        {
            waveText = FindText("waves", "Waves", "WaveNum", "WaveNumber");
        }

        if (counterText == null)
        {
            counterText = FindText("counter", "Counter", "WaveCounter", "Timer");
        }
    }

    public void BindSpawner(EnemySpawner spawner)
    {
        enemySpawner = spawner;
        Refresh(true);
    }

    public void Refresh(bool force)
    {
        if (enemySpawner == null)
        {
            return;
        }

        int wave = Mathf.Max(1, enemySpawner.CurrentWave);
        int counter = Mathf.Max(0, Mathf.CeilToInt(enemySpawner.CurrentWaveRemainingSeconds));

        if (waveText != null && (force || wave != lastDisplayedWave))
        {
            waveText.text = wave.ToString();
            lastDisplayedWave = wave;
        }

        if (counterText != null && (force || counter != lastDisplayedCounter))
        {
            counterText.text = FormatCounter(counter);
            lastDisplayedCounter = counter;
        }
    }

    private string FormatCounter(int seconds)
    {
        if (!useMinuteSecondCounter)
        {
            return seconds.ToString();
        }

        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return $"{minutes:00}:{remainingSeconds:00}";
    }

    private TMP_Text FindText(params string[] names)
    {
        TMP_Text selfText = GetComponent<TMP_Text>();
        if (selfText != null && Matches(selfText.name, names))
        {
            return selfText;
        }

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            if (text != null && Matches(text.name, names))
            {
                return text;
            }
        }

        return null;
    }

    private static bool Matches(string value, string[] names)
    {
        foreach (string name in names)
        {
            if (value == name)
            {
                return true;
            }
        }

        return false;
    }
}
