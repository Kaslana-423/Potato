using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerHealthBarView : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private bool bindPlayerHealthOnEnable = true;
    [SerializeField] private bool addPlayerHealthIfMissing = true;

    [Header("UI")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text currentHealthText;
    [SerializeField] private TMP_Text maxHealthText;

    private bool subscribed;

    private void Awake()
    {
        AutoBindReferences();
    }

    private void Start()
    {
        if (bindPlayerHealthOnEnable && playerHealth == null)
        {
            BindPlayerHealth(ResolvePlayerHealth());
        }

        Refresh();
    }

    private void OnEnable()
    {
        AutoBindReferences();

        if (bindPlayerHealthOnEnable)
        {
            BindPlayerHealth(playerHealth != null ? playerHealth : ResolvePlayerHealth());
        }

        Refresh();
    }

    private void OnDisable()
    {
        UnbindPlayerHealth();
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
        Transform bloodRoot = FindDescendant("Blood");

        if (fillImage == null)
        {
            Transform fill = FindDescendantUnder(bloodRoot, "Fill", "fill");
            fillImage = fill != null ? fill.GetComponent<Image>() : null;
        }

        if (currentHealthText == null)
        {
            currentHealthText = FindComponentUnder<TMP_Text>(
                bloodRoot,
                "CurrentHealth",
                "Current Health",
                "CurrentHp",
                "Current HP");
        }

        if (maxHealthText == null)
        {
            maxHealthText = FindComponentUnder<TMP_Text>(
                bloodRoot,
                "MaxHealth",
                "Max Health",
                "MaxHp",
                "Max HP");
        }
    }

    public void BindPlayerHealth(PlayerHealth newPlayerHealth)
    {
        if (playerHealth != newPlayerHealth)
        {
            UnbindPlayerHealth();
            playerHealth = newPlayerHealth;
        }

        if (playerHealth != null && !subscribed)
        {
            playerHealth.HealthChanged += HandleHealthChanged;
            subscribed = true;
        }

        Refresh();
    }

    public void Refresh()
    {
        if (playerHealth == null)
        {
            return;
        }

        Refresh(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void HandleHealthChanged(PlayerHealth health, int currentHealth, int maxHealth, int delta)
    {
        Refresh(currentHealth, maxHealth);
    }

    private void Refresh(int currentHealth, int maxHealth)
    {
        int safeMaxHealth = Mathf.Max(1, maxHealth);
        int safeCurrentHealth = Mathf.Clamp(currentHealth, 0, safeMaxHealth);

        if (fillImage != null)
        {
            fillImage.fillAmount = (float)safeCurrentHealth / safeMaxHealth;
        }

        if (currentHealthText != null)
        {
            currentHealthText.text = safeCurrentHealth.ToString();
        }

        if (maxHealthText != null)
        {
            maxHealthText.text = safeMaxHealth.ToString();
        }
    }

    private void UnbindPlayerHealth()
    {
        if (playerHealth != null && subscribed)
        {
            playerHealth.HealthChanged -= HandleHealthChanged;
        }

        subscribed = false;
    }

    private PlayerHealth ResolvePlayerHealth()
    {
        PlayerHealth foundHealth = playerHealth;
        if (foundHealth != null)
        {
            return foundHealth;
        }

        if (PlayerStats.Instance != null)
        {
            foundHealth = PlayerStats.Instance.GetComponent<PlayerHealth>();
            if (foundHealth == null && addPlayerHealthIfMissing)
            {
                foundHealth = PlayerStats.Instance.gameObject.AddComponent<PlayerHealth>();
            }
        }

        if (foundHealth != null)
        {
            return foundHealth;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return Object.FindObjectOfType<PlayerHealth>(true);
        }

        foundHealth = player.GetComponent<PlayerHealth>();
        if (foundHealth == null && addPlayerHealthIfMissing)
        {
            foundHealth = player.AddComponent<PlayerHealth>();
        }

        return foundHealth != null ? foundHealth : Object.FindObjectOfType<PlayerHealth>(true);
    }

    private Transform FindDescendant(params string[] names)
    {
        return FindDescendantUnder(transform, names);
    }

    private Transform FindDescendantUnder(Transform root, params string[] names)
    {
        if (root == null)
        {
            return null;
        }

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (string.Equals(child.name, objectName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }
            }
        }

        return null;
    }

    private T FindComponentUnder<T>(Transform root, params string[] names) where T : Component
    {
        Transform child = FindDescendantUnder(root != null ? root : transform, names);
        return child != null ? child.GetComponent<T>() : null;
    }
}
