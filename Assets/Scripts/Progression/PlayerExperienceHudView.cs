using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerExperienceHudView : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerExperience experience;
    [SerializeField] private bool bindExperienceOnEnable = true;

    [Header("UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private Image fillImage;

    private PlayerExperience subscribedExperience;

    public bool HasSceneReferences => experience != null
        && levelText != null
        && experienceText != null
        && fillImage != null;

    private void Awake()
    {
        AutoBindReferences();
    }

    private void OnEnable()
    {
        AutoBindReferences();
        if (bindExperienceOnEnable)
        {
            Bind(experience);
        }

        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public static PlayerExperienceHudView FindAndBind(PlayerExperience playerExperience)
    {
        Transform playerState = FindPlayerState();
        PlayerExperienceHudView existing = playerState != null
            ? playerState.GetComponent<PlayerExperienceHudView>()
            : FindObjectOfType<PlayerExperienceHudView>(true);

        if (existing == null)
        {
            Debug.LogError("SampleScene is missing its PlayerExperienceHudView scene component.");
            return null;
        }

        existing.EnsureUi();
        existing.Bind(playerExperience);
        return existing;
    }

    public void Bind(PlayerExperience playerExperience)
    {
        if (subscribedExperience != playerExperience)
        {
            Unsubscribe();
            subscribedExperience = playerExperience;
            if (subscribedExperience != null)
            {
                subscribedExperience.ExperienceChanged += HandleExperienceChanged;
            }
        }

        experience = playerExperience;
        Refresh();
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        if (experience == null)
        {
            experience = FindObjectOfType<PlayerExperience>(true);
        }

        BindUiReferences();
    }

    private void HandleExperienceChanged(PlayerExperience changedExperience)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (experience == null || levelText == null || experienceText == null || fillImage == null)
        {
            return;
        }

        levelText.text = $"Lv.{experience.Level}";
        experienceText.text = $"{experience.CurrentExperienceDisplay} / {experience.ExperienceRequired}";
        fillImage.fillAmount = experience.Progress;
    }

    private void EnsureUi()
    {
        AutoBindReferences();
        if (levelText == null || experienceText == null || fillImage == null)
        {
            Debug.LogError("PlayerState experience UI references are incomplete in SampleScene.", this);
        }
    }

    private void BindUiReferences()
    {
        Transform root = transform.Find("Experience");
        if (root == null)
        {
            return;
        }

        levelText = root.Find("Level")?.GetComponent<TMP_Text>();
        experienceText = root.Find("Value")?.GetComponent<TMP_Text>()
            ?? root.Find("Bar/Value")?.GetComponent<TMP_Text>();
        fillImage = root.Find("Fill")?.GetComponent<Image>()
            ?? root.Find("Bar/Fill")?.GetComponent<Image>();
    }

    private void Unsubscribe()
    {
        if (subscribedExperience != null)
        {
            subscribedExperience.ExperienceChanged -= HandleExperienceChanged;
            subscribedExperience = null;
        }
    }

    private static Transform FindPlayerState()
    {
        PlayerHealthBarView healthView = FindObjectOfType<PlayerHealthBarView>(true);
        if (healthView != null)
        {
            return healthView.transform;
        }

        GameObject playerState = GameObject.Find("PlayerState");
        return playerState != null ? playerState.transform : null;
    }

}
