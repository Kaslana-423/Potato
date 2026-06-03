using System;
using System.Collections.Generic;

public static class ShopLocalization
{
    private static readonly IReadOnlyDictionary<string, string> contentNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["weapon.brick_dlc.tier_1"] = "砖块 (DLC)",
            ["weapon.cacti_club.tier_1"] = "仙人掌棍",
            ["weapon.chopper.tier_1"] = "砍刀",
            ["weapon.claw.tier_1"] = "爪",
            ["weapon.knife.tier_1"] = "小刀",
            ["item.acid"] = "强酸",
            ["item.adrenaline"] = "肾上腺素",
            ["item.alien_baby"] = "外星宝宝",
            ["item.alien_magic"] = "外星魔法",
            ["item.alien_tongue"] = "外星舌头"
        };

    private static readonly IReadOnlyDictionary<string, string> contentDescriptions =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["weapon.brick_dlc.tier_1"] = "命中时有 1% 概率破碎并掉落材料。",
            ["weapon.cacti_club.tier_1"] = "命中敌人时发射 3 枚造成 50% 伤害的投射物。",
            ["weapon.chopper.tier_1"] = "适合近距离战斗的快速刀刃。",
            ["weapon.claw.tier_1"] = "攻击间隔较短的精准徒手武器。",
            ["weapon.knife.tier_1"] = "轻巧而精准的快速近战武器。",
            ["item.acid"] = "用机动性与击退能力换取更多生命值。",
            ["item.adrenaline"] = "闪避攻击时有 50% 概率恢复 5 点生命值。",
            ["item.alien_baby"] = "敌人的生命值提高 10%。",
            ["item.alien_magic"] = "外星能量强化了身体，但会降低幸运。",
            ["item.alien_tongue"] = "更容易拾取远处的材料。"
        };

    private static readonly IReadOnlyDictionary<string, string> statNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Max HP"] = "最大生命值",
            ["HP Regeneration"] = "生命再生",
            ["Life Steal"] = "生命窃取",
            ["Damage"] = "伤害",
            ["Melee Damage"] = "近战伤害",
            ["Ranged Damage"] = "远程伤害",
            ["Elemental Damage"] = "元素伤害",
            ["Explosion Damage"] = "爆炸伤害",
            ["Piercing Damage"] = "穿透伤害",
            ["Attack Speed"] = "攻击速度",
            ["Crit Chance"] = "暴击率",
            ["Engineering"] = "工程学",
            ["Range"] = "攻击范围",
            ["Armor"] = "护甲",
            ["Dodge"] = "闪避",
            ["Speed"] = "速度",
            ["Luck"] = "幸运",
            ["Harvesting"] = "收获",
            ["Knockback"] = "击退",
            ["Pickup Range"] = "拾取范围",
            ["XP Gain"] = "经验获取",
            ["Items Price"] = "道具价格",
            ["Curse"] = "诅咒"
        };

    private static readonly IReadOnlyDictionary<string, string> weaponClasses =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Blunt"] = "钝器",
            ["Primitive"] = "原始",
            ["Heavy"] = "重型",
            ["Blade"] = "刀刃",
            ["Unarmed"] = "徒手",
            ["Precise"] = "精准",
            ["Support"] = "辅助",
            ["Ethereal"] = "虚灵",
            ["Medieval"] = "中世纪",
            ["Naval"] = "海军",
            ["Tool"] = "工具",
            ["Medical"] = "医疗",
            ["Elemental"] = "元素",
            ["Explosive"] = "爆炸",
            ["Gun"] = "枪械",
            ["Laser"] = "激光"
        };

    public static string GetContentName(string id, string fallback)
    {
        return contentNames.TryGetValue(id, out string value) ? value : fallback;
    }

    public static string GetContentDescription(string id, string fallback)
    {
        return contentDescriptions.TryGetValue(id, out string value) ? value : fallback;
    }

    public static string GetKindLabel(ShopContentKind kind)
    {
        return kind == ShopContentKind.Weapon ? "武器" : "道具";
    }

    public static string GetRarityLabel(ShopRarity rarity)
    {
        return $"{(int)rarity} 级";
    }

    public static string GetStatName(string statName)
    {
        return statNames.TryGetValue(statName, out string value) ? value : statName;
    }

    public static string GetWeaponClasses(string classTags)
    {
        if (string.IsNullOrWhiteSpace(classTags))
        {
            return string.Empty;
        }

        string[] tags = classTags.Split(',');
        for (int index = 0; index < tags.Length; index++)
        {
            string tag = tags[index].Trim();
            tags[index] = weaponClasses.TryGetValue(tag, out string value) ? value : tag;
        }

        return string.Join("、", tags);
    }
}
