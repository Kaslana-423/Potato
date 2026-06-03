using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStatId
{
    Level,
    MaxHp,
    HpRegeneration,
    LifeSteal,
    Damage,
    MeleeDamage,
    RangedDamage,
    ElementalDamage,
    AttackSpeed,
    CritChance,
    Engineering,
    Range,
    Armor,
    Dodge,
    Speed,
    Luck,
    Harvesting
}

public sealed class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public event Action<PlayerStats> StatsChanged;

    [Header("主要属性")]
    [SerializeField, Min(1)] private int level = 23;
    [SerializeField] private int maxHp = 53;
    [SerializeField] private int hpRegeneration = 0;
    [SerializeField] private int lifeSteal = 5;
    [SerializeField] private int damage = -17;
    [SerializeField] private int meleeDamage = 5;
    [SerializeField] private int rangedDamage = 8;
    [SerializeField] private int elementalDamage = 2;
    [SerializeField] private int attackSpeed = 23;
    [SerializeField] private int critChance = 59;
    [SerializeField] private int engineering = 8;
    [SerializeField] private int range = 171;
    [SerializeField] private int armor = 3;
    [SerializeField] private int dodge = 12;
    [SerializeField] private int speed = 8;
    [SerializeField] private int luck = 22;
    [SerializeField] private int harvesting = 18;

    public int Level => level;
    public int MaxHp => maxHp;
    public int HpRegeneration => hpRegeneration;
    public int LifeSteal => lifeSteal;
    public int Damage => damage;
    public int MeleeDamage => meleeDamage;
    public int RangedDamage => rangedDamage;
    public int ElementalDamage => elementalDamage;
    public int AttackSpeed => attackSpeed;
    public int CritChance => critChance;
    public int Engineering => engineering;
    public int Range => range;
    public int Armor => armor;
    public int Dodge => dodge;
    public int Speed => speed;
    public int Luck => luck;
    public int Harvesting => harvesting;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Scene already has a PlayerStats singleton. Disabling duplicate component.", this);
            enabled = false;
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        NotifyStatsChanged();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnValidate()
    {
        level = Mathf.Max(1, level);

        if (Application.isPlaying && Instance == this)
        {
            NotifyStatsChanged();
        }
    }

    public void SetLevel(int newLevel)
    {
        int clampedLevel = Mathf.Max(1, newLevel);
        if (level == clampedLevel)
        {
            return;
        }

        level = clampedLevel;
        NotifyStatsChanged();
    }

    public void AddLevel(int amount)
    {
        SetLevel(level + amount);
    }

    public int GetStat(PlayerStatId id)
    {
        switch (id)
        {
            case PlayerStatId.Level:
                return level;
            case PlayerStatId.MaxHp:
                return maxHp;
            case PlayerStatId.HpRegeneration:
                return hpRegeneration;
            case PlayerStatId.LifeSteal:
                return lifeSteal;
            case PlayerStatId.Damage:
                return damage;
            case PlayerStatId.MeleeDamage:
                return meleeDamage;
            case PlayerStatId.RangedDamage:
                return rangedDamage;
            case PlayerStatId.ElementalDamage:
                return elementalDamage;
            case PlayerStatId.AttackSpeed:
                return attackSpeed;
            case PlayerStatId.CritChance:
                return critChance;
            case PlayerStatId.Engineering:
                return engineering;
            case PlayerStatId.Range:
                return range;
            case PlayerStatId.Armor:
                return armor;
            case PlayerStatId.Dodge:
                return dodge;
            case PlayerStatId.Speed:
                return speed;
            case PlayerStatId.Luck:
                return luck;
            case PlayerStatId.Harvesting:
                return harvesting;
            default:
                return 0;
        }
    }

    public int GetStat(string id)
    {
        return TryParseStatId(id, out PlayerStatId parsedId) ? GetStat(parsedId) : 0;
    }

    public void SetStat(PlayerStatId id, int value)
    {
        if (id == PlayerStatId.Level)
        {
            SetLevel(value);
            return;
        }

        if (GetStat(id) == value)
        {
            return;
        }

        switch (id)
        {
            case PlayerStatId.MaxHp:
                maxHp = value;
                break;
            case PlayerStatId.HpRegeneration:
                hpRegeneration = value;
                break;
            case PlayerStatId.LifeSteal:
                lifeSteal = value;
                break;
            case PlayerStatId.Damage:
                damage = value;
                break;
            case PlayerStatId.MeleeDamage:
                meleeDamage = value;
                break;
            case PlayerStatId.RangedDamage:
                rangedDamage = value;
                break;
            case PlayerStatId.ElementalDamage:
                elementalDamage = value;
                break;
            case PlayerStatId.AttackSpeed:
                attackSpeed = value;
                break;
            case PlayerStatId.CritChance:
                critChance = value;
                break;
            case PlayerStatId.Engineering:
                engineering = value;
                break;
            case PlayerStatId.Range:
                range = value;
                break;
            case PlayerStatId.Armor:
                armor = value;
                break;
            case PlayerStatId.Dodge:
                dodge = value;
                break;
            case PlayerStatId.Speed:
                speed = value;
                break;
            case PlayerStatId.Luck:
                luck = value;
                break;
            case PlayerStatId.Harvesting:
                harvesting = value;
                break;
        }

        NotifyStatsChanged();
    }

    public void SetStat(PlayerStatId id, float value)
    {
        SetStat(id, Mathf.RoundToInt(value));
    }

    public bool SetStat(string id, int value)
    {
        if (!TryParseStatId(id, out PlayerStatId parsedId))
        {
            Debug.LogWarning($"Unknown player stat id: {id}", this);
            return false;
        }

        SetStat(parsedId, value);
        return true;
    }

    public bool SetStat(string id, float value)
    {
        if (!TryParseStatId(id, out PlayerStatId parsedId))
        {
            Debug.LogWarning($"Unknown player stat id: {id}", this);
            return false;
        }

        SetStat(parsedId, Mathf.RoundToInt(value));
        return true;
    }

    public void AddStat(PlayerStatId id, int amount)
    {
        SetStat(id, GetStat(id) + amount);
    }

    public void AddStat(PlayerStatId id, float amount)
    {
        AddStat(id, Mathf.RoundToInt(amount));
    }

    public bool AddStat(string id, int amount)
    {
        if (!TryParseStatId(id, out PlayerStatId parsedId))
        {
            Debug.LogWarning($"Unknown player stat id: {id}", this);
            return false;
        }

        AddStat(parsedId, amount);
        return true;
    }

    public bool AddStat(string id, float amount)
    {
        if (!TryParseStatId(id, out PlayerStatId parsedId))
        {
            Debug.LogWarning($"Unknown player stat id: {id}", this);
            return false;
        }

        AddStat(parsedId, Mathf.RoundToInt(amount));
        return true;
    }

    public IReadOnlyList<PlayerStatDisplayEntry> BuildDisplayEntries()
    {
        return new List<PlayerStatDisplayEntry>
        {
            MakeEntry(PlayerStatId.Level, "当前等级", "级", level, new Color(0.82f, 0.92f, 1f, 1f), true),
            MakeEntry(PlayerStatId.MaxHp, "最大生命值", "心", maxHp, new Color(0.20f, 0.95f, 0.35f, 1f)),
            MakeEntry(PlayerStatId.HpRegeneration, "生命再生", "生", hpRegeneration, new Color(0.35f, 1f, 0.35f, 1f)),
            MakeEntry(PlayerStatId.LifeSteal, "生命窃取", "窃", lifeSteal, new Color(0.95f, 0.25f, 0.32f, 1f)),
            MakeEntry(PlayerStatId.Damage, "伤害", "伤", damage, new Color(1f, 0.20f, 0.25f, 1f)),
            MakeEntry(PlayerStatId.MeleeDamage, "近战伤害", "近", meleeDamage, new Color(0.95f, 0.88f, 0.45f, 1f)),
            MakeEntry(PlayerStatId.RangedDamage, "远程伤害", "远", rangedDamage, new Color(0.78f, 0.45f, 1f, 1f)),
            MakeEntry(PlayerStatId.ElementalDamage, "元素伤害", "元", elementalDamage, new Color(1f, 0.58f, 0.35f, 1f)),
            MakeEntry(PlayerStatId.AttackSpeed, "攻击速度", "速", attackSpeed, new Color(0.90f, 0.90f, 0.90f, 1f)),
            MakeEntry(PlayerStatId.CritChance, "暴击率", "暴", critChance, new Color(1f, 0.20f, 0.25f, 1f)),
            MakeEntry(PlayerStatId.Engineering, "工程学", "工", engineering, new Color(0.35f, 0.95f, 1f, 1f)),
            MakeEntry(PlayerStatId.Range, "范围", "范", range, new Color(0.78f, 0.45f, 1f, 1f)),
            MakeEntry(PlayerStatId.Armor, "护甲", "护", armor, new Color(0.98f, 0.88f, 0.20f, 1f)),
            MakeEntry(PlayerStatId.Dodge, "闪避", "闪", dodge, new Color(0.70f, 0.95f, 1f, 1f)),
            MakeEntry(PlayerStatId.Speed, "速度", "移", speed, new Color(0.92f, 0.92f, 0.92f, 1f)),
            MakeEntry(PlayerStatId.Luck, "幸运", "运", luck, new Color(0.98f, 0.98f, 0.98f, 1f)),
            MakeEntry(PlayerStatId.Harvesting, "收获", "收", harvesting, new Color(1f, 0.90f, 0.48f, 1f)),
        };
    }

    public void NotifyStatsChanged()
    {
        StatsChanged?.Invoke(this);
    }

    public static bool TryParseStatId(string id, out PlayerStatId parsedId)
    {
        parsedId = PlayerStatId.Level;
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        string normalized = NormalizeId(id);
        switch (normalized)
        {
            case "level":
            case "currentlevel":
                parsedId = PlayerStatId.Level;
                return true;
            case "maxhp":
            case "maxhealth":
            case "maximumhealth":
                parsedId = PlayerStatId.MaxHp;
                return true;
            case "hpregeneration":
            case "healthregeneration":
            case "regen":
                parsedId = PlayerStatId.HpRegeneration;
                return true;
            case "lifesteal":
                parsedId = PlayerStatId.LifeSteal;
                return true;
            case "damage":
                parsedId = PlayerStatId.Damage;
                return true;
            case "meleedamage":
                parsedId = PlayerStatId.MeleeDamage;
                return true;
            case "rangeddamage":
                parsedId = PlayerStatId.RangedDamage;
                return true;
            case "elementaldamage":
                parsedId = PlayerStatId.ElementalDamage;
                return true;
            case "attackspeed":
                parsedId = PlayerStatId.AttackSpeed;
                return true;
            case "critchance":
            case "criticalchance":
                parsedId = PlayerStatId.CritChance;
                return true;
            case "engineering":
                parsedId = PlayerStatId.Engineering;
                return true;
            case "range":
                parsedId = PlayerStatId.Range;
                return true;
            case "armor":
                parsedId = PlayerStatId.Armor;
                return true;
            case "dodge":
                parsedId = PlayerStatId.Dodge;
                return true;
            case "speed":
                parsedId = PlayerStatId.Speed;
                return true;
            case "luck":
                parsedId = PlayerStatId.Luck;
                return true;
            case "harvesting":
            case "harvest":
                parsedId = PlayerStatId.Harvesting;
                return true;
            default:
                return false;
        }
    }

    private static PlayerStatDisplayEntry MakeEntry(
        PlayerStatId id,
        string displayName,
        string iconText,
        int value,
        Color iconColor,
        bool alwaysWhite = false)
    {
        Color statColor = alwaysWhite ? Color.white : GetValueColor(value);
        return new PlayerStatDisplayEntry
        {
            Id = GetDisplayId(id),
            DisplayName = displayName,
            IconText = iconText,
            IconSprite = null,
            ValueText = FormatStatValue(value),
            IconColor = iconColor,
            NameColor = alwaysWhite ? PlayerStatsPanelView.SoftWhite : statColor,
            ValueColor = statColor,
        };
    }

    private static Color GetValueColor(float value)
    {
        if (value > 0f)
        {
            return PlayerStatsPanelView.PositiveGreen;
        }

        if (value < 0f)
        {
            return PlayerStatsPanelView.NegativeRed;
        }

        return Color.white;
    }

    private static string FormatStatValue(int value)
    {
        return value.ToString();
    }

    private static string GetDisplayId(PlayerStatId id)
    {
        switch (id)
        {
            case PlayerStatId.Level:
                return "level";
            case PlayerStatId.MaxHp:
                return "max_hp";
            case PlayerStatId.HpRegeneration:
                return "hp_regeneration";
            case PlayerStatId.LifeSteal:
                return "life_steal";
            case PlayerStatId.Damage:
                return "damage";
            case PlayerStatId.MeleeDamage:
                return "melee_damage";
            case PlayerStatId.RangedDamage:
                return "ranged_damage";
            case PlayerStatId.ElementalDamage:
                return "elemental_damage";
            case PlayerStatId.AttackSpeed:
                return "attack_speed";
            case PlayerStatId.CritChance:
                return "crit_chance";
            case PlayerStatId.Engineering:
                return "engineering";
            case PlayerStatId.Range:
                return "range";
            case PlayerStatId.Armor:
                return "armor";
            case PlayerStatId.Dodge:
                return "dodge";
            case PlayerStatId.Speed:
                return "speed";
            case PlayerStatId.Luck:
                return "luck";
            case PlayerStatId.Harvesting:
                return "harvesting";
            default:
                return id.ToString();
        }
    }

    private static string NormalizeId(string id)
    {
        char[] buffer = new char[id.Length];
        int count = 0;
        foreach (char character in id)
        {
            if (char.IsLetterOrDigit(character))
            {
                buffer[count] = char.ToLowerInvariant(character);
                count++;
            }
        }

        return new string(buffer, 0, count);
    }
}
