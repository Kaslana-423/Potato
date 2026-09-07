using System;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterCatalog
{
    private const string ResourcesPath = "Characters";

    private static IReadOnlyList<CharacterDefinition> all;

    public static IReadOnlyList<CharacterDefinition> All
    {
        get
        {
            if (all == null)
            {
                all = BuildCatalog();
            }

            return all;
        }
    }

    public static CharacterDefinition FindById(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return null;
        }

        IReadOnlyList<CharacterDefinition> characters = All;
        for (int index = 0; index < characters.Count; index++)
        {
            CharacterDefinition character = characters[index];
            if (string.Equals(character.Id, characterId, StringComparison.OrdinalIgnoreCase))
            {
                return character;
            }
        }

        return null;
    }

    public static bool TryGetById(string characterId, out CharacterDefinition character)
    {
        character = FindById(characterId);
        return character != null;
    }

    public static int IndexOf(string characterId)
    {
        IReadOnlyList<CharacterDefinition> characters = All;
        for (int index = 0; index < characters.Count; index++)
        {
            if (string.Equals(characters[index].Id, characterId, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        return -1;
    }

    public static void Reload()
    {
        all = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeCache()
    {
        all = null;
    }

    private static IReadOnlyList<CharacterDefinition> BuildCatalog()
    {
        CharacterDefinition[] loaded = Resources.LoadAll<CharacterDefinition>(ResourcesPath);
        var characters = new List<CharacterDefinition>(loaded.Length);
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < loaded.Length; index++)
        {
            CharacterDefinition character = loaded[index];
            if (character == null
                || !character.VisibleInSelection
                || string.IsNullOrWhiteSpace(character.Id)
                || !ids.Add(character.Id))
            {
                continue;
            }

            characters.Add(character);
        }

        characters.Sort(CompareCharacters);
        return characters;
    }

    private static int CompareCharacters(CharacterDefinition left, CharacterDefinition right)
    {
        int orderComparison = left.DisplayOrder.CompareTo(right.DisplayOrder);
        return orderComparison != 0
            ? orderComparison
            : string.Compare(left.Id, right.Id, StringComparison.OrdinalIgnoreCase);
    }
}
