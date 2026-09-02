using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerExperience : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField, Min(0f)] private float currentExperience;
    [SerializeField] private bool resetExperienceOnAwake = true;

    private readonly List<int> pendingUpgradeLevels = new List<int>();
    private PlayerStats playerStats;

    public event Action<PlayerExperience> ExperienceChanged;

    public float CurrentExperience => currentExperience;
    public int CurrentExperienceDisplay => Mathf.FloorToInt(currentExperience);
    public int ExperienceRequired => GetExperienceRequiredForNextLevel(Level);
    public int Level => playerStats != null ? playerStats.Level : 1;
    public int PendingUpgradeCount => pendingUpgradeLevels.Count;
    public IReadOnlyList<int> PendingUpgradeLevels => pendingUpgradeLevels;
    public float Progress => ExperienceRequired > 0
        ? Mathf.Clamp01(currentExperience / ExperienceRequired)
        : 0f;

    private void Awake()
    {
        ResolvePlayerStats();
        if (resetExperienceOnAwake)
        {
            currentExperience = 0f;
            pendingUpgradeLevels.Clear();
        }
    }

    private void OnValidate()
    {
        currentExperience = Mathf.Max(0f, currentExperience);
    }

    public static PlayerExperience GetOrCreate()
    {
        PlayerExperience existing = FindObjectOfType<PlayerExperience>(true);
        if (existing != null)
        {
            return existing;
        }

        GameObject owner = PlayerStats.Instance != null ? PlayerStats.Instance.gameObject : null;
        if (owner == null)
        {
            owner = GameObject.FindGameObjectWithTag("Player");
        }

        return owner != null ? owner.AddComponent<PlayerExperience>() : null;
    }

    public void AddMaterialExperience(int materialAmount)
    {
        if (materialAmount <= 0)
        {
            return;
        }

        ResolvePlayerStats();
        float experienceMultiplier = playerStats != null
            ? Mathf.Max(0f, 1f + playerStats.XpGain / 100f)
            : 1f;
        AddExperience(materialAmount * experienceMultiplier);
    }

    public void AddExperience(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        ResolvePlayerStats();
        currentExperience += amount;

        while (playerStats != null && currentExperience >= GetExperienceRequiredForNextLevel(playerStats.Level))
        {
            int required = GetExperienceRequiredForNextLevel(playerStats.Level);
            currentExperience -= required;
            playerStats.AddLevel(1);
            playerStats.AddStat(PlayerStatId.MaxHp, 1);
            pendingUpgradeLevels.Add(playerStats.Level);
        }

        ExperienceChanged?.Invoke(this);
    }

    public bool TryPeekPendingUpgrade(out int level)
    {
        if (pendingUpgradeLevels.Count == 0)
        {
            level = 0;
            return false;
        }

        level = pendingUpgradeLevels[0];
        return true;
    }

    public bool TryConsumePendingUpgrade(int expectedLevel)
    {
        if (pendingUpgradeLevels.Count == 0 || pendingUpgradeLevels[0] != expectedLevel)
        {
            return false;
        }

        pendingUpgradeLevels.RemoveAt(0);
        ExperienceChanged?.Invoke(this);
        return true;
    }

    public void RestoreState(float experience, IReadOnlyList<int> pendingLevels)
    {
        ResolvePlayerStats();
        currentExperience = Mathf.Max(0f, experience);
        pendingUpgradeLevels.Clear();
        if (pendingLevels != null)
        {
            for (int index = 0; index < pendingLevels.Count; index++)
            {
                pendingUpgradeLevels.Add(Mathf.Max(1, pendingLevels[index]));
            }
        }

        ExperienceChanged?.Invoke(this);
    }

    public static int GetExperienceRequiredForNextLevel(int currentLevel)
    {
        int adjustedLevel = Mathf.Max(1, currentLevel) + 3;
        return adjustedLevel * adjustedLevel;
    }

    private void ResolvePlayerStats()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (playerStats == null)
        {
            playerStats = PlayerStats.Instance;
        }
    }
}
