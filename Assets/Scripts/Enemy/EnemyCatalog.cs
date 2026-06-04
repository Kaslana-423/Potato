using System;
using System.Collections.Generic;
using System.Linq;

public static class EnemyCatalog
{
    private static readonly IReadOnlyList<EnemyDefinition> all = BuildCatalog();
    private static readonly int[] waveDurations =
    {
        20, 25, 30, 35, 40,
        45, 50, 55, 60, 60,
        60, 60, 60, 60, 60,
        60, 60, 60, 60, 90
    };

    public static IReadOnlyList<EnemyDefinition> All => all;

    public static bool TryGetById(string id, out EnemyDefinition enemy)
    {
        enemy = null;
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        for (int index = 0; index < all.Count; index++)
        {
            if (string.Equals(all[index].Id, id, StringComparison.OrdinalIgnoreCase))
            {
                enemy = all[index];
                return true;
            }
        }

        return false;
    }

    public static IEnumerable<EnemyDefinition> GetEligibleEnemies(
        int wave,
        bool includeDlcEnemies,
        bool includeElites,
        bool includeBosses)
    {
        int clampedWave = Math.Max(1, wave);
        return all.Where(enemy =>
            enemy.FirstWave <= clampedWave
            && (includeDlcEnemies || !enemy.IsDlc)
            && (includeElites || !enemy.IsElite)
            && (includeBosses || !enemy.IsBoss));
    }

    public static int GetWaveDurationSeconds(int wave)
    {
        int index = Math.Max(1, wave) - 1;
        return index < waveDurations.Length ? waveDurations[index] : waveDurations[waveDurations.Length - 1];
    }

    private static IReadOnlyList<EnemyDefinition> BuildCatalog()
    {
        var contents = new List<EnemyDefinition>();
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (EnemyDefinition enemy in GeneratedEnemyCatalog.CreateAll())
        {
            AddUnique(contents, ids, enemy);
        }

        return contents;
    }

    private static void AddUnique(
        ICollection<EnemyDefinition> contents,
        ISet<string> ids,
        EnemyDefinition enemy)
    {
        if (enemy != null && ids.Add(enemy.Id))
        {
            contents.Add(enemy);
        }
    }
}
