using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelUpRewardController : MonoBehaviour
{
    private sealed class OptionView
    {
        public Button Button;
        public Image Background;
        public TMP_Text TierText;
        public TMP_Text NameText;
        public TMP_Text ValueText;
        public TMP_Text CurrentText;
    }

    private readonly List<OptionView> optionViews = new List<OptionView>();
    private IReadOnlyList<LevelUpUpgradeOption> currentOptions = Array.Empty<LevelUpUpgradeOption>();
    [Header("Scene UI References")]
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text pendingText;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TMP_Text rerollText;
    [SerializeField] private PlayerStatsPanelView statsPanelView;
    private PlayerExperience experience;
    private PlayerStats playerStats;
    private PlayerWallet wallet;
    private int currentRewardLevel;
    private int wave;
    private int paidRerollCount;

    public bool IsProcessing { get; private set; }

    private void Awake()
    {
        AutoBindReferences();
        BindSceneButtons();
        if (!HasSceneReferences())
        {
            Debug.LogError("Level-up reward UI is missing from SampleScene. Rebuild it with Tools/Potato UI/Build Reward Panels In SampleScene.", this);
        }
        SetVisible(false);
    }

    private void OnDestroy()
    {
        BindWallet(null);
        UnbindSceneButtons();
    }

    public static LevelUpRewardController FindInScene()
    {
        LevelUpRewardController existing = FindObjectOfType<LevelUpRewardController>(true);
        if (existing != null)
        {
            return existing;
        }

        Debug.LogError("LevelUpRewardController must be placed in the gameplay scene.");
        return null;
    }

    public void BeginRewards(PlayerExperience playerExperience, int currentWave)
    {
        if (!HasSceneReferences())
        {
            CompleteRewards();
            return;
        }

        experience = playerExperience;
        playerStats = PlayerStats.Instance;
        wave = Mathf.Max(1, currentWave);
        paidRerollCount = 0;

        if (experience == null || playerStats == null || experience.PendingUpgradeCount <= 0)
        {
            CompleteRewards();
            return;
        }

        BindWallet(PlayerWallet.GetOrCreate());
        IsProcessing = true;
        SetVisible(true);
        statsPanelView?.BindPlayerStats(playerStats);
        ShowNextReward();
    }

    private void ShowNextReward()
    {
        if (experience == null || !experience.TryPeekPendingUpgrade(out currentRewardLevel))
        {
            CompleteRewards();
            return;
        }

        int luck = playerStats != null ? playerStats.Luck : 0;
        currentOptions = LevelUpUpgradeCatalog.GenerateOptions(currentRewardLevel, luck, optionViews.Count);
        titleText.text = $"升级奖励 · 等级 {currentRewardLevel}";
        pendingText.text = experience.PendingUpgradeCount > 1
            ? $"选择一项属性强化（剩余 {experience.PendingUpgradeCount} 次）"
            : "选择一项属性强化";

        for (int index = 0; index < optionViews.Count; index++)
        {
            OptionView view = optionViews[index];
            bool hasOption = index < currentOptions.Count;
            view.Button.gameObject.SetActive(hasOption);
            if (!hasOption)
            {
                continue;
            }

            LevelUpUpgradeOption option = currentOptions[index];
            Color tierColor = LevelUpUpgradeCatalog.GetTierColor(option.Tier);
            view.Background.color = tierColor;
            view.TierText.text = LevelUpUpgradeCatalog.GetTierLabel(option.Tier);
            view.NameText.text = option.DisplayName;
            view.ValueText.text = $"+{option.Value}{GetValueSuffix(option.StatId)}";
            int currentValue = playerStats.GetStat(option.StatId);
            view.CurrentText.text = $"当前 {currentValue}  →  {currentValue + option.Value}";
        }

        UpdateRerollButton();
    }

    private void SelectOption(int optionIndex)
    {
        if (!IsProcessing || optionIndex < 0 || optionIndex >= currentOptions.Count)
        {
            return;
        }

        LevelUpUpgradeOption option = currentOptions[optionIndex];
        playerStats.AddStat(option.StatId, option.Value);
        if (!experience.TryConsumePendingUpgrade(currentRewardLevel))
        {
            Debug.LogWarning("Level-up reward queue changed before the selected reward was consumed.", this);
            CompleteRewards();
            return;
        }

        ShowNextReward();
    }

    private void Reroll()
    {
        if (!IsProcessing || wallet == null)
        {
            return;
        }

        int cost = GetRerollCost();
        if (!wallet.TrySpend(cost))
        {
            UpdateRerollButton();
            return;
        }

        paidRerollCount++;
        ShowNextReward();
    }

    private int GetRerollCost()
    {
        int baseCost = wave + wave / 2;
        int increase = Mathf.Max(1, wave / 2);
        int rawCost = baseCost + paidRerollCount * increase;
        int modifier = playerStats != null ? playerStats.RerollPrice : 0;
        return Mathf.Max(1, Mathf.FloorToInt(rawCost * Mathf.Max(0f, 1f + modifier / 100f)));
    }

    private void UpdateRerollButton()
    {
        if (rerollButton == null || rerollText == null)
        {
            return;
        }

        int cost = GetRerollCost();
        bool affordable = wallet != null && wallet.CanSpend(cost);
        rerollButton.interactable = affordable;
        rerollText.text = $"重随  {cost}";
        rerollText.color = affordable
            ? new Color(0.38f, 1f, 0.4f, 1f)
            : new Color(1f, 0.32f, 0.32f, 1f);
    }

    private void CompleteRewards()
    {
        IsProcessing = false;
        SetVisible(false);
        BindWallet(null);
    }

    private void HandleCoinsChanged(PlayerWallet changedWallet, int coins, int delta)
    {
        UpdateRerollButton();
    }

    private void BindWallet(PlayerWallet newWallet)
    {
        if (wallet == newWallet)
        {
            return;
        }

        if (wallet != null)
        {
            wallet.CoinsChanged -= HandleCoinsChanged;
        }

        wallet = newWallet;
        if (wallet != null)
        {
            wallet.CoinsChanged += HandleCoinsChanged;
        }
    }

    private void SetVisible(bool visible)
    {
        if (windowRoot != null && windowRoot.activeSelf != visible)
        {
            windowRoot.SetActive(visible);
        }
    }

    [ContextMenu("Auto Bind Scene References")]
    public void AutoBindReferences()
    {
        windowRoot = FindDescendant("LevelUpWindow")?.gameObject;
        titleText = FindComponent<TMP_Text>("Title");
        pendingText = FindComponent<TMP_Text>("Pending");
        rerollButton = FindComponent<Button>("RerollButton");
        rerollText = rerollButton != null ? rerollButton.GetComponentInChildren<TMP_Text>(true) : null;
        statsPanelView = FindComponent<PlayerStatsPanelView>("UpgradeStatsPanel");

        optionViews.Clear();
        Transform optionsRoot = FindDescendant("Options");
        if (optionsRoot == null)
        {
            return;
        }

        for (int index = 0; index < optionsRoot.childCount; index++)
        {
            Transform optionRoot = optionsRoot.GetChild(index);
            if (optionRoot.name != "UpgradeOption")
            {
                continue;
            }

            optionViews.Add(new OptionView
            {
                Button = optionRoot.GetComponent<Button>(),
                Background = optionRoot.GetComponent<Image>(),
                TierText = FindChildComponent<TMP_Text>(optionRoot, "Tier"),
                NameText = FindChildComponent<TMP_Text>(optionRoot, "Name"),
                ValueText = FindChildComponent<TMP_Text>(optionRoot, "Value"),
                CurrentText = FindChildComponent<TMP_Text>(optionRoot, "Current"),
            });
        }
    }

    private void BindSceneButtons()
    {
        if (rerollButton != null)
        {
            rerollButton.onClick.RemoveListener(Reroll);
            rerollButton.onClick.AddListener(Reroll);
        }

        for (int index = 0; index < optionViews.Count; index++)
        {
            int capturedIndex = index;
            Button button = optionViews[index].Button;
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectOption(capturedIndex));
            }
        }
    }

    private void UnbindSceneButtons()
    {
        if (rerollButton != null)
        {
            rerollButton.onClick.RemoveListener(Reroll);
        }
        foreach (OptionView view in optionViews)
        {
            view.Button?.onClick.RemoveAllListeners();
        }
    }

    private bool HasSceneReferences()
    {
        if (windowRoot == null || titleText == null || pendingText == null || rerollButton == null
            || rerollText == null || statsPanelView == null || optionViews.Count != 4)
        {
            return false;
        }
        return optionViews.TrueForAll(view => view.Button != null && view.Background != null
            && view.TierText != null && view.NameText != null && view.ValueText != null && view.CurrentText != null);
    }

    private Transform FindDescendant(params string[] names)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (child.name == objectName)
                {
                    return child;
                }
            }
        }
        return null;
    }

    private T FindComponent<T>(params string[] names) where T : Component
    {
        Transform child = FindDescendant(names);
        return child != null ? child.GetComponent<T>() : null;
    }

    private static T FindChildComponent<T>(Transform root, string objectName) where T : Component
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == objectName)
            {
                return child.GetComponent<T>();
            }
        }
        return null;
    }

    private static string GetValueSuffix(PlayerStatId statId)
    {
        switch (statId)
        {
            case PlayerStatId.LifeSteal:
            case PlayerStatId.Damage:
            case PlayerStatId.AttackSpeed:
            case PlayerStatId.CritChance:
            case PlayerStatId.Dodge:
            case PlayerStatId.Speed:
                return "%";
            default:
                return string.Empty;
        }
    }

}
