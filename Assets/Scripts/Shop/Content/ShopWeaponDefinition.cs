using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

public enum WeaponAttackStyle
{
    Unspecified,
    Slash,
    Thrust,
    Ranged
}

public readonly struct WeaponDamageScaling
{
    public WeaponDamageScaling(PlayerStatId statId, float percentage)
    {
        StatId = statId;
        Percentage = percentage;
    }

    public PlayerStatId StatId { get; }
    public float Percentage { get; }
}

public abstract class ShopWeaponDefinition : ShopContentDefinition
{
    private WeaponDamageScaling[] cachedDamageScalings;

    public sealed override ShopContentKind Kind => ShopContentKind.Weapon;

    public virtual WeaponAttackStyle AttackStyle => WeaponAttackStyle.Unspecified;
    public virtual float Damage => 0f;
    public virtual string DamageScalingText => string.Empty;
    public virtual string DamageScalingStats => string.Empty;
    public virtual float AttackCooldown => 1f;
    public virtual float AttackRange => 0f;
    public virtual float CritMultiplier => 1.5f;
    public virtual float CritChance => 0f;
    public virtual float Knockback => 0f;
    public virtual float LifeSteal => 0f;
    public virtual string ClassTags => string.Empty;
    public virtual string SpecialEffects => string.Empty;
    public virtual string RuntimePrefabResourcePath => string.Empty;
    public virtual string RuntimeSpriteResourcePath => string.Empty;

    public override string IconResourcePath
    {
        get
        {
            switch (AttackStyle)
            {
                case WeaponAttackStyle.Thrust:
                    return "Weapon/spear_icon";
                case WeaponAttackStyle.Ranged:
                    return "Weapon/sword_icon";
                default:
                    return "Weapon/sword_icon";
            }
        }
    }

    public string FamilyId => GetFamilyId(Id);
    public string LocalizedClassTags => ShopLocalization.GetWeaponClasses(ClassTags);
    public IReadOnlyList<WeaponDamageScaling> DamageScalings =>
        cachedDamageScalings ?? (cachedDamageScalings = ParseDamageScalings());

    public static string GetFamilyId(string weaponId)
    {
        if (string.IsNullOrWhiteSpace(weaponId))
        {
            return string.Empty;
        }

        int tierMarkerIndex = weaponId.LastIndexOf(".tier_", StringComparison.OrdinalIgnoreCase);
        return tierMarkerIndex > 0 ? weaponId.Substring(0, tierMarkerIndex) : weaponId;
    }

    public float CalculateDamage(PlayerStats stats)
    {
        float result = Damage;
        if (stats == null)
        {
            return result;
        }

        foreach (WeaponDamageScaling scaling in DamageScalings)
        {
            result += stats.GetStat(scaling.StatId) * scaling.Percentage / 100f;
        }

        return result;
    }

    public override string BuildStatLine()
    {
        string scaling = string.IsNullOrWhiteSpace(DamageScalingText)
            ? string.Empty
            : $" ({DamageScalingText})";
        return string.Format(
            CultureInfo.InvariantCulture,
            "<color=#55E875>{0:0.##}</color>{1} 伤害\n<color=#55E875>{2:0.##}</color> 秒攻击间隔\n<color=#55E875>{3:0.##}</color> 攻击范围\n<color=#55E875>{4:0.##}%</color> 暴击率 · <color=#55E875>x{5:0.##}</color> 暴击伤害\n<color=#55E875>{6:0.##}</color> 击退 · <color=#55E875>{7:0.##}%</color> 生命窃取",
            Damage,
            scaling,
            AttackCooldown,
            AttackRange,
            CritChance,
            CritMultiplier,
            Knockback,
            LifeSteal);
    }

    private WeaponDamageScaling[] ParseDamageScalings()
    {
        if (string.IsNullOrWhiteSpace(DamageScalingText)
            || string.IsNullOrWhiteSpace(DamageScalingStats))
        {
            return Array.Empty<WeaponDamageScaling>();
        }

        string[] statNames = DamageScalingStats.Split(',');
        MatchCollection percentageMatches = Regex.Matches(DamageScalingText, @"[+-]?\d+(?:\.\d+)?");
        var scalings = new List<WeaponDamageScaling>(Math.Min(statNames.Length, percentageMatches.Count));
        int count = Math.Min(statNames.Length, percentageMatches.Count);
        for (int index = 0; index < count; index++)
        {
            if (!PlayerStats.TryParseStatId(statNames[index], out PlayerStatId statId)
                || !float.TryParse(
                    percentageMatches[index].Value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float percentage))
            {
                continue;
            }

            scalings.Add(new WeaponDamageScaling(statId, percentage));
        }

        return scalings.ToArray();
    }
}
