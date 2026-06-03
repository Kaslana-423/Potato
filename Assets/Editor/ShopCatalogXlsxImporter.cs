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
        EditorUtility.DisplayDialog(
            "Potato Shop",
            $"Generated {weaponCount} weapon scripts and {itemCount} item scripts.",
            "OK");
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
            source.AppendLine($"    public override float Damage => {ToFloatLiteral(Get(row, "Damage"))};");
            source.AppendLine($"    public override float AttackCooldown => {ToFloatLiteral(Get(row, "Attack Speed (s)"), 1f)};");
            source.AppendLine($"    public override float AttackRange => {ToFloatLiteral(Get(row, "Range"))};");
            source.AppendLine($"    public override string ClassTags => {ToLiteral(Get(row, "Class"))};");
            source.AppendLine($"    public override string SpecialEffects => {ToLiteral(Get(row, "Special Effects"))};");
            source.AppendLine("}");

            WriteGeneratedFile(outputDirectory, $"{className}.generated.cs", source.ToString());
            registryEntries.Add(className);
            count++;
        }

        return count;
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

        return modifiers;
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
