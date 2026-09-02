using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveContext
{
    public const int SlotCount = 3;

    private const string SlotDirectoryPrefix = "save_slot_";
    private const string SaveFileName = "save.json";
    private const string BackupFileName = "save.backup.json";
    private const string TemporaryFileName = "save.tmp";

    public static event Action CurrentSaveChanged;

    public static SaveData CurrentSave { get; private set; }
    public static bool HasCurrentSave => CurrentSave != null;
    public static int CurrentSlotId => HasCurrentSave ? CurrentSave.slotId : 0;
    public static string CurrentSaveDirectory => HasCurrentSave
        ? GetSlotDirectory(CurrentSave.slotId)
        : string.Empty;

    public static SaveSlotInfo GetSlotInfo(int slotId)
    {
        if (!IsValidSlot(slotId))
        {
            return new SaveSlotInfo(slotId, false, false, string.Empty, string.Empty);
        }

        bool hasFiles = HasSaveFiles(slotId);
        if (!TryLoadSave(slotId, out SaveData saveData, false))
        {
            return new SaveSlotInfo(slotId, hasFiles, !hasFiles, $"存档 {slotId}", string.Empty);
        }

        return new SaveSlotInfo(
            slotId,
            true,
            true,
            string.IsNullOrWhiteSpace(saveData.displayName) ? $"存档 {slotId}" : saveData.displayName,
            saveData.lastPlayedAtUtc);
    }

    public static bool SelectOrCreateSave(int slotId)
    {
        if (!IsValidSlot(slotId))
        {
            Debug.LogError($"Invalid save slot: {slotId}");
            return false;
        }

        SaveData saveData;
        if (!TryLoadSave(slotId, out saveData, true))
        {
            if (HasSaveFiles(slotId))
            {
                Debug.LogError($"Save slot {slotId} is corrupted and was not overwritten.");
                return false;
            }

            string now = DateTime.UtcNow.ToString("O");
            saveData = new SaveData
            {
                slotId = slotId,
                saveId = Guid.NewGuid().ToString("N"),
                displayName = $"存档 {slotId}",
                createdAtUtc = now,
                lastPlayedAtUtc = now
            };
        }
        else
        {
            saveData.lastPlayedAtUtc = DateTime.UtcNow.ToString("O");
        }

        if (!TryWriteSave(saveData))
        {
            return false;
        }

        CurrentSave = saveData;
        CurrentSaveChanged?.Invoke();
        return true;
    }

    public static void ClearCurrentSave()
    {
        if (CurrentSave == null)
        {
            return;
        }

        CurrentSave = null;
        CurrentSaveChanged?.Invoke();
    }

    public static string GetSlotDirectory(int slotId)
    {
        if (!IsValidSlot(slotId))
        {
            throw new ArgumentOutOfRangeException(nameof(slotId));
        }

        return Path.Combine(Application.persistentDataPath, SlotDirectoryPrefix + slotId);
    }

    private static bool TryLoadSave(int slotId, out SaveData saveData, bool repairMainFile)
    {
        string directory = GetSlotDirectory(slotId);
        string mainPath = Path.Combine(directory, SaveFileName);
        if (TryReadSave(mainPath, slotId, out saveData))
        {
            return true;
        }

        string backupPath = Path.Combine(directory, BackupFileName);
        string temporaryPath = Path.Combine(directory, TemporaryFileName);
        if (!TryReadSave(backupPath, slotId, out saveData)
            && !TryReadSave(temporaryPath, slotId, out saveData))
        {
            saveData = null;
            return false;
        }

        if (repairMainFile)
        {
            TryWriteSave(saveData);
        }

        return true;
    }

    private static bool TryReadSave(string path, int expectedSlotId, out SaveData saveData)
    {
        saveData = null;
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(path, Encoding.UTF8));
            return saveData != null
                && saveData.version == SaveData.CurrentVersion
                && saveData.slotId == expectedSlotId
                && !string.IsNullOrWhiteSpace(saveData.saveId);
        }
        catch (Exception)
        {
            saveData = null;
            return false;
        }
    }

    private static bool TryWriteSave(SaveData saveData)
    {
        saveData.version = SaveData.CurrentVersion;
        string directory = GetSlotDirectory(saveData.slotId);
        string mainPath = Path.Combine(directory, SaveFileName);
        string backupPath = Path.Combine(directory, BackupFileName);
        string temporaryPath = Path.Combine(directory, TemporaryFileName);

        try
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(temporaryPath, JsonUtility.ToJson(saveData, true), new UTF8Encoding(false));

            if (File.Exists(mainPath))
            {
                try
                {
                    File.Replace(temporaryPath, mainPath, backupPath, true);
                }
                catch (PlatformNotSupportedException)
                {
                    CopySaveFileFallback(mainPath, backupPath, temporaryPath);
                }
                catch (IOException)
                {
                    CopySaveFileFallback(mainPath, backupPath, temporaryPath);
                }
            }
            else
            {
                File.Move(temporaryPath, mainPath);
            }

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Save metadata could not be written to '{mainPath}': {exception.Message}");
            return false;
        }
    }

    private static void CopySaveFileFallback(string mainPath, string backupPath, string temporaryPath)
    {
        File.Copy(mainPath, backupPath, true);
        File.Copy(temporaryPath, mainPath, true);
        File.Delete(temporaryPath);
    }

    private static bool HasSaveFiles(int slotId)
    {
        string directory = GetSlotDirectory(slotId);
        return File.Exists(Path.Combine(directory, SaveFileName))
            || File.Exists(Path.Combine(directory, BackupFileName))
            || File.Exists(Path.Combine(directory, TemporaryFileName));
    }

    private static bool IsValidSlot(int slotId)
    {
        return slotId >= 1 && slotId <= SlotCount;
    }
}
