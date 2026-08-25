using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public static class ShopCatalogXlsxImporter
{
    private const string OutputAssetDirectory = "Assets/Scripts/Shop/Generated";
    private const string RegistryFileName = "GeneratedShopContentCatalog.generated.cs";

    private static readonly string[] ItemStatHeaders =
    {
        "Max HP",
        "HP Regeneration",
        "Life Steal %",
        "Damage %",
        "Melee Damage",
        "Ranged Damage",
        "Elemental Damage",
        "Explosion Damage %",
        "Piercing Damage %",
        "Attack Speed %",
        "Crit Chance %",
        "Engineering",
        "Range",
        "Armor",
        "Dodge %",
        "Speed %",
        "Luck",
        "Harvesting",
        "Knockback",
        "Pickup Range %",
        "XP Gain %",
        "Items Price %",
        "Curse"
    };

    [MenuItem("Tools/Potato Shop/Generate Scripts From XLSX")]
    public static void GenerateScriptsFromXlsx()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string weaponsPath = Path.Combine(projectRoot, "weapons.xlsx");
        string itemsPath = Path.Combine(projectRoot, "items.xlsx");
        string outputDirectory = Path.Combine(projectRoot, OutputAssetDirectory);

        if (!File.Exists(weaponsPath) || !File.Exists(itemsPath))
        {
            EditorUtility.DisplayDialog(
                "Potato Shop",
                "Expected weapons.xlsx and items.xlsx in the project root.",
                "OK");
            return;
        }

        Directory.CreateDirectory(outputDirectory);
        foreach (string generatedFile in Directory.GetFiles(outputDirectory, "*.generated.cs"))
        {
            File.Delete(generatedFile);
        }

        var registryEntries = new List<string>();
        var usedClassNames = new HashSet<string>(StringComparer.Ordinal);

        int weaponCount = GenerateWeapons(weaponsPath, outputDirectory, registryEntries, usedClassNames);
        int itemCount = GenerateItems(itemsPath, outputDirectory, registryEntries, usedClassNames);

        WriteGeneratedFile(
            outputDirectory,
            RegistryFileName,
            BuildRegistrySource(registryEntries));

        AssetDatabase.Refresh();
        string resultMessage = $"Generated {weaponCount} weapon scripts and {itemCount} item scripts.";
        if (Application.isBatchMode)
        {
            Debug.Log(resultMessage);
        }
        else
        {
            EditorUtility.DisplayDialog("Potato Shop", resultMessage, "OK");
        }
    }

    private static int GenerateWeapons(
        string xlsxPath,
        string outputDirectory,
        ICollection<string> registryEntries,
        ISet<string> usedClassNames)
    {
        int count = 0;
        foreach (Dictionary<string, string> row in ReadRows(xlsxPath))
        {
            string name = Get(row, "Name");
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            int tier = Mathf.Clamp(ParseInt(Get(row, "Tier"), 1), 1, 4);
            string className = MakeUniqueClassName(
                $"{ToIdentifier(name)}Tier{tier}GeneratedWeapon",
                usedClassNames);
            string id = $"weapon.{ToSlug(name)}.tier_{tier}";

            var source = new StringBuilder();
            source.AppendLine($"public sealed class {className} : ShopWeaponDefinition");
            source.AppendLine("{");
            source.AppendLine($"    public override string Id => {ToLiteral(id)};");
            source.AppendLine($"    public override string DisplayName => {ToLiteral(name)};");
            source.AppendLine($"    public override string Description => {ToLiteral(Get(row, "Special Effects"))};");
            source.AppendLine($"    public override int BasePrice => {ParseInt(Get(row, "Base price"), 0)};");
            source.AppendLine($"    public override ShopRarity Rarity => ShopRarity.Tier{tier};");
            source.AppendLine($"    public override WeaponAttackStyle AttackStyle => WeaponAttackStyle.{ResolveWeaponAttackStyle(name)};");
            source.AppendLine($"    public override float Damage => {ToFloatLiteral(Get(row, "Damage"))};");
            source.AppendLine($"    public override string DamageScalingText => {ToLiteral(Get(row, "Damage Scaling"))};");
            source.AppendLine($"    public override string DamageScalingStats => {ToLiteral(ResolveWeaponScalingStats(name))};");
            source.AppendLine($"    public override float AttackCooldown => {ToFloatLiteral(Get(row, "Attack Speed (s)"), 1f)};");
            source.AppendLine($"    public override float AttackRange => {ToFloatLiteral(Get(row, "Range"))};");
            source.AppendLine($"    public override float CritMultiplier => {ToFloatLiteral(Get(row, "Crit Mult"), 1.5f)};");
            source.AppendLine($"    public override float CritChance => {ToFloatLiteral(Get(row, "Crit Chance %"))};");
            source.AppendLine($"    public override float Knockback => {ToFloatLiteral(Get(row, "Knockback"))};");
            string lifeSteal = string.IsNullOrWhiteSpace(Get(row, "Lifesteal %"))
                ? Get(row, "Lifesteal")
                : Get(row, "Lifesteal %");
            source.AppendLine($"    public override float LifeSteal => {ToFloatLiteral(lifeSteal)};");
            source.AppendLine($"    public override string ClassTags => {ToLiteral(Get(row, "Class"))};");
            source.AppendLine($"    public override string SpecialEffects => {ToLiteral(Get(row, "Special Effects"))};");
            if (string.Equals(name, "Stick", StringComparison.OrdinalIgnoreCase))
            {
                source.AppendLine("    public override string IconResourcePath => \"Weapon/stick\";");
                source.AppendLine("    public override string RuntimePrefabResourcePath => \"Weapon/Prefabs/StartingStickWeapon\";");
                source.AppendLine("    public override string RuntimeSpriteResourcePath => \"Weapon/stick\";");
            }
            source.AppendLine("}");

            WriteGeneratedFile(outputDirectory, $"{className}.generated.cs", source.ToString());
            registryEntries.Add(className);
            count++;
        }

        return count;
    }

    private static WeaponAttackStyle ResolveWeaponAttackStyle(string weaponName)
    {
        switch ((weaponName ?? string.Empty).Trim())
        {
            case "Claw":
            case "Drill":
            case "Fist":
            case "Flaming Brass Knuckles":
            case "Ghost Flint":
            case "Hand":
            case "Hiking Pole (DLC)":
            case "Jousting Lance":
            case "Knife":
            case "Lightning Shiv":
            case "Power Fist":
            case "Pruner":
            case "Quarterstaff":
            case "Scissors":
            case "Screwdriver":
            case "Sharp Tooth":
            case "Spear":
            case "Spiky Shield":
            case "Stick":
            case "Thief Dagger":
            case "Trident (DLC)":
                return WeaponAttackStyle.Thrust;
            default:
                return WeaponAttackStyle.Slash;
        }
    }

    private static string ResolveWeaponScalingStats(string weaponName)
    {
        switch ((weaponName ?? string.Empty).Trim())
        {
            case "Anchor (DLC)": return "Melee Damage,Curse";
            case "Brick (DLC)": return "Melee Damage,Engineering";
            case "Captain's Sword (DLC)": return "Melee Damage,Curse";
            case "Chainsaw (DLC)": return "Melee Damage,Engineering,Life Steal";
            case "Chopper": return "Melee Damage,Max HP";
            case "Claw": return "Attack Speed,Melee Damage";
            case "DEX-troyer": return "Melee Damage,Engineering";
            case "Drill": return "Melee Damage,Engineering";
            case "Excalibur": return "Melee Damage,Max HP";
            case "Hatchet": return "Melee Damage,Attack Speed";
            case "Hiking Pole (DLC)": return "Melee Damage,Range";
            case "Jousting Lance": return "Melee Damage,Speed";
            case "Lute (DLC)": return "Melee Damage,Luck";
            case "Mace (DLC)": return "Melee Damage,Attack Speed";
            case "Plank": return "Melee Damage,Elemental Damage,Engineering";
            case "Plasma Sledge": return "Melee Damage,Elemental Damage";
            case "Quarterstaff": return "Level,Melee Damage";
            case "Screwdriver": return "Melee Damage,Engineering";
            case "Scythe": return "Melee Damage,Life Steal";
            case "Sharp Tooth": return "Melee Damage,Life Steal";
            case "Sickle (DLC)": return "Melee Damage,Harvesting";
            case "Spiky Shield": return "Armor";
            case "Spoon (DLC)": return "Melee Damage,Max HP";
            case "Thunder Sword": return "Melee Damage,Elemental Damage";
            case "Torch": return "Melee Damage,Elemental Damage";
            case "Trident (DLC)": return "Melee Damage,Curse";
            case "War Hammer (DLC)": return "Melee Damage,Engineering";
            default: return "Melee Damage";
        }
    }

    private static int GenerateItems(
        string xlsxPath,
        string outputDirectory,
        ICollection<string> registryEntries,
        ISet<string> usedClassNames)
    {
        int count = 0;
        foreach (Dictionary<string, string> row in ReadRows(xlsxPath))
        {
            string name = Get(row, "Name");
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            int tier = ParseTier(Get(row, "Rarity"));
            string className = MakeUniqueClassName(
                $"{ToIdentifier(name)}GeneratedItem",
                usedClassNames);
            string id = $"item.{ToSlug(name)}";
            List<string> modifiers = BuildModifierInitializers(row);

            var source = new StringBuilder();
            source.AppendLine("using System.Collections.Generic;");
            source.AppendLine();
            source.AppendLine($"public sealed class {className} : ShopItemDefinition");
            source.AppendLine("{");
            source.AppendLine("    private static readonly ItemStatModifier[] modifiers =");
            source.AppendLine("    {");
            foreach (string modifier in modifiers)
            {
                source.AppendLine($"        {modifier},");
            }
            source.AppendLine("    };");
            source.AppendLine();
            source.AppendLine($"    public override string Id => {ToLiteral(id)};");
            source.AppendLine($"    public override string DisplayName => {ToLiteral(name)};");
            source.AppendLine($"    public override string Description => {ToLiteral(Get(row, "Effects"))};");
            source.AppendLine($"    public override int BasePrice => {ParseInt(Get(row, "Base Price"), 0)};");
            source.AppendLine($"    public override ShopRarity Rarity => ShopRarity.Tier{tier};");
            source.AppendLine($"    public override int PurchaseLimit => {ParseInt(Get(row, "Limit"), 0)};");
            source.AppendLine("    public override IReadOnlyList<ItemStatModifier> Modifiers => modifiers;");
            source.AppendLine("}");

            WriteGeneratedFile(outputDirectory, $"{className}.generated.cs", source.ToString());
            registryEntries.Add(className);
            count++;
        }

        return count;
    }

    private static List<string> BuildModifierInitializers(IReadOnlyDictionary<string, string> row)
    {
        var modifiers = new List<string>();
        foreach (string header in ItemStatHeaders)
        {
            string rawValue = Get(row, header);
            if (!TryParseFloat(rawValue, out float value) || Mathf.Approximately(value, 0f))
            {
                continue;
            }

            bool isPercent = header.Contains("%");
            string statName = header.Replace(" %", string.Empty);
            modifiers.Add(
                $"new ItemStatModifier({ToLiteral(statName)}, {ToFloatLiteral(value)}, {isPercent.ToString().ToLowerInvariant()})");
        }

        AppendDescriptionDerivedModifiers(Get(row, "Effects"), modifiers);

        return modifiers;
    }

    private static void AppendDescriptionDerivedModifiers(string effects, ICollection<string> modifiers)
    {
        if (string.IsNullOrWhiteSpace(effects))
        {
            return;
        }

        AddMatchedModifier(effects, @"([+-]\s*\d+(?:\.\d+)?)\s*HP recovered from consumables", "Consumable Heal", false, modifiers);
        AddMatchedModifier(effects, @"([+-]\s*\d+(?:\.\d+)?)\s*%\s*Explosion Size", "Explosion Size", true, modifiers);
        AddMatchedModifier(effects, @"Projectiles pierce through\s*(\d+(?:\.\d+)?)\s*additional target", "Piercing", false, modifiers);
        AddMatchedModifier(effects, @"projectiles gain\s*([+-]\s*\d+(?:\.\d+)?)\s*bounce", "Bounces", false, modifiers);
        AddMatchedModifier(effects, @"([+-]\s*\d+(?:\.\d+)?)\s*free reroll", "Free Rerolls", false, modifiers);
        AddMatchedModifier(effects, @"([+-]\s*\d+(?:\.\d+)?)\s*%\s*Structure attack speed", "Structure Attack Speed", true, modifiers);
        AddMatchedModifier(effects, @"([+-]\s*\d+(?:\.\d+)?)\s*%\s*Enemy Speed(?!\s+during the next wave)", "Enemy Speed", true, modifiers);
        AddMatchedModifier(effects, @"([+-]\s*\d+(?:\.\d+)?)\s*%\s*Enemies(?:\s|$)", "Enemies", true, modifiers);
        AddMatchedModifier(effects, @"([+-]\s*\d+(?:\.\d+)?)\s*%\s*Reroll Price", "Reroll Price", true, modifiers);
        AddMatchedModifier(effects, @"([+-]?\s*\d+(?:\.\d+)?)\s*%\s*chance to double the value of picked up materials", "Double Material Chance", true, modifiers);
        AddMatchedModifier(effects, @"([+-]?\s*\d+(?:\.\d+)?)\s*%\s*chance to heal 1 HP when picking up a material", "Materials Healing", true, modifiers);

        Match bossDamage = Regex.Match(
            effects,
            @"([+-]\s*\d+(?:\.\d+)?)\s*%\s*damage against bosses and elites",
            RegexOptions.IgnoreCase);
        if (bossDamage.Success && TryParseMatchedFloat(bossDamage.Groups[1].Value, out float bossDamageValue))
        {
            RemoveModifier(modifiers, "Damage");
            AddModifierIfMissing(modifiers, "Damage Against Bosses", bossDamageValue, true);
        }

        if (Regex.IsMatch(effects, @"Burning spreads to an additional nearby enemy", RegexOptions.IgnoreCase))
        {
            AddModifierIfMissing(modifiers, "Burning Spread", 1f, false);
        }

        Match burningSpeed = Regex.Match(
            effects,
            @"Burning activates\s*(\d+(?:\.\d+)?)\s*%\s*(faster|slower)",
            RegexOptions.IgnoreCase);
        if (burningSpeed.Success && TryParseMatchedFloat(burningSpeed.Groups[1].Value, out float burningValue))
        {
            if (string.Equals(burningSpeed.Groups[2].Value, "slower", StringComparison.OrdinalIgnoreCase))
            {
                burningValue = -burningValue;
            }

            AddModifierIfMissing(modifiers, "Burning Speed", burningValue, true);
        }

        if (Regex.IsMatch(effects, @"\bMore trees spawn\b", RegexOptions.IgnoreCase))
        {
            AddModifierIfMissing(modifiers, "Trees", 1f, false);
        }
    }

    private static void AddMatchedModifier(
        string effects,
        string pattern,
        string statName,
        bool isPercent,
        ICollection<string> modifiers)
    {
        Match match = Regex.Match(effects, pattern, RegexOptions.IgnoreCase);
        if (match.Success && TryParseMatchedFloat(match.Groups[1].Value, out float value))
        {
            AddModifierIfMissing(modifiers, statName, value, isPercent);
        }
    }

    private static bool TryParseMatchedFloat(string value, out float parsed)
    {
        return TryParseFloat((value ?? string.Empty).Replace(" ", string.Empty), out parsed);
    }

    private static void AddModifierIfMissing(
        ICollection<string> modifiers,
        string statName,
        float value,
        bool isPercent)
    {
        string statLiteral = ToLiteral(statName);
        if (modifiers.Any(modifier => modifier.Contains(statLiteral)))
        {
            return;
        }

        modifiers.Add(
            $"new ItemStatModifier({statLiteral}, {ToFloatLiteral(value)}, {isPercent.ToString().ToLowerInvariant()})");
    }

    private static void RemoveModifier(ICollection<string> modifiers, string statName)
    {
        string statLiteral = ToLiteral(statName);
        string existing = modifiers.FirstOrDefault(modifier => modifier.Contains(statLiteral));
        if (existing != null)
        {
            modifiers.Remove(existing);
        }
    }

    private static IEnumerable<Dictionary<string, string>> ReadRows(string xlsxPath)
    {
        using (ZipArchive archive = ZipFile.OpenRead(xlsxPath))
        {
            XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            List<string> sharedStrings = ReadSharedStrings(archive, ns);
            ZipArchiveEntry sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml");
            if (sheetEntry == null)
            {
                yield break;
            }

            XDocument sheet;
            using (Stream stream = sheetEntry.Open())
            {
                sheet = XDocument.Load(stream);
            }

            List<XElement> rows = sheet.Descendants(ns + "row").ToList();
            if (rows.Count == 0)
            {
                yield break;
            }

            Dictionary<int, string> headers = ReadCells(rows[0], sharedStrings, ns);
            foreach (XElement row in rows.Skip(1))
            {
                Dictionary<int, string> cells = ReadCells(row, sharedStrings, ns);
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (KeyValuePair<int, string> header in headers)
                {
                    values[header.Value] = cells.TryGetValue(header.Key, out string value)
                        ? value
                        : string.Empty;
                }

                yield return values;
            }
        }
    }

    private static List<string> ReadSharedStrings(ZipArchive archive, XNamespace ns)
    {
        var sharedStrings = new List<string>();
        ZipArchiveEntry entry = archive.GetEntry("xl/sharedStrings.xml");
        if (entry == null)
        {
            return sharedStrings;
        }

        XDocument document;
        using (Stream stream = entry.Open())
        {
            document = XDocument.Load(stream);
        }

        foreach (XElement item in document.Descendants(ns + "si"))
        {
            sharedStrings.Add(string.Concat(item.Descendants(ns + "t").Select(text => text.Value)));
        }

        return sharedStrings;
    }

    private static Dictionary<int, string> ReadCells(
        XElement row,
        IReadOnlyList<string> sharedStrings,
        XNamespace ns)
    {
        var cells = new Dictionary<int, string>();
        foreach (XElement cell in row.Elements(ns + "c"))
        {
            string reference = (string)cell.Attribute("r");
            int columnIndex = GetColumnIndex(reference);
            string type = (string)cell.Attribute("t");
            string value;

            if (type == "inlineStr")
            {
                value = string.Concat(cell.Descendants(ns + "t").Select(text => text.Value));
            }
            else
            {
                value = (string)cell.Element(ns + "v") ?? string.Empty;
                if (type == "s" && int.TryParse(value, out int sharedStringIndex))
                {
                    value = sharedStringIndex >= 0 && sharedStringIndex < sharedStrings.Count
                        ? sharedStrings[sharedStringIndex]
                        : string.Empty;
                }
            }

            cells[columnIndex] = value;
        }

        return cells;
    }

    private static int GetColumnIndex(string reference)
    {
        int result = 0;
        foreach (char character in reference)
        {
            if (!char.IsLetter(character))
            {
                break;
            }

            result = result * 26 + char.ToUpperInvariant(character) - 'A' + 1;
        }

        return result - 1;
    }

    private static string BuildRegistrySource(IEnumerable<string> classNames)
    {
        var source = new StringBuilder();
        source.AppendLine("using System.Collections.Generic;");
        source.AppendLine();
        source.AppendLine("public static class GeneratedShopContentCatalog");
        source.AppendLine("{");
        source.AppendLine("    public static IEnumerable<ShopContentDefinition> CreateAll()");
        source.AppendLine("    {");

        bool hasEntries = false;
        foreach (string className in classNames)
        {
            source.AppendLine($"        yield return new {className}();");
            hasEntries = true;
        }

        if (!hasEntries)
        {
            source.AppendLine("        yield break;");
        }

        source.AppendLine("    }");
        source.AppendLine("}");
        return source.ToString();
    }

    private static void WriteGeneratedFile(string outputDirectory, string fileName, string contents)
    {
        File.WriteAllText(
            Path.Combine(outputDirectory, fileName),
            contents,
            new UTF8Encoding(false));
    }

    private static int ParseTier(string value)
    {
        Match match = Regex.Match(value ?? string.Empty, @"\d+");
        return match.Success
            ? Mathf.Clamp(ParseInt(match.Value, 1), 1, 4)
            : 1;
    }

    private static int ParseInt(string value, int fallback)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
            ? parsed
            : fallback;
    }

    private static bool TryParseFloat(string value, out float parsed)
    {
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed);
    }

    private static string ToFloatLiteral(string value, float fallback = 0f)
    {
        return TryParseFloat(value, out float parsed)
            ? ToFloatLiteral(parsed)
            : ToFloatLiteral(fallback);
    }

    private static string ToFloatLiteral(float value)
    {
        return value.ToString("0.########", CultureInfo.InvariantCulture) + "f";
    }

    private static string Get(IReadOnlyDictionary<string, string> row, string key)
    {
        return row.TryGetValue(key, out string value) ? value : string.Empty;
    }

    private static string MakeUniqueClassName(string baseName, ISet<string> usedClassNames)
    {
        string className = baseName;
        int suffix = 2;
        while (!usedClassNames.Add(className))
        {
            className = baseName + suffix;
            suffix++;
        }

        return className;
    }

    private static string ToIdentifier(string value)
    {
        var identifier = new StringBuilder();
        bool capitalizeNext = true;
        foreach (char character in value)
        {
            if (!IsAsciiLetterOrDigit(character))
            {
                capitalizeNext = true;
                continue;
            }

            char output = capitalizeNext ? char.ToUpperInvariant(character) : character;
            identifier.Append(output);
            capitalizeNext = false;
        }

        if (identifier.Length == 0 || char.IsDigit(identifier[0]))
        {
            identifier.Insert(0, "Content");
        }

        return identifier.ToString();
    }

    private static string ToSlug(string value)
    {
        var slug = new StringBuilder();
        bool pendingSeparator = false;
        foreach (char character in value.ToLowerInvariant())
        {
            if (IsAsciiLetterOrDigit(character))
            {
                if (pendingSeparator && slug.Length > 0)
                {
                    slug.Append('_');
                }

                slug.Append(character);
                pendingSeparator = false;
            }
            else
            {
                pendingSeparator = true;
            }
        }

        return slug.Length > 0 ? slug.ToString() : "content";
    }

    private static bool IsAsciiLetterOrDigit(char character)
    {
        return character >= 'a' && character <= 'z'
            || character >= 'A' && character <= 'Z'
            || character >= '0' && character <= '9';
    }

    private static string ToLiteral(string value)
    {
        string escaped = (value ?? string.Empty)
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
        return $"\"{escaped}\"";
    }
}
