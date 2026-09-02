using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class GameSessionState
{
    private const string LegacyActiveRunKey = "potato.session.active";
    private const string LegacyRunDataKey = "potato.session.run_data";
    private const string MasterVolumeKey = "potato.settings.master_volume";
    private const string FullscreenKey = "potato.settings.fullscreen";
    private const string RunSaveFileName = "run_save.json";
    private const string RunSaveBackupFileName = "run_save.backup.json";
    private const string RunSaveTemporaryFileName = "run_save.tmp";

    public static string RunSavePath => Path.Combine(Application.persistentDataPath, RunSaveFileName);
    public static string RunSaveBackupPath => Path.Combine(Application.persistentDataPath, RunSaveBackupFileName);
    private static string RunSaveTemporaryPath => Path.Combine(Application.persistentDataPath, RunSaveTemporaryFileName);

    public static bool HasActiveRun => HasValidFileSave()
        || TryReadLegacySave(out _);
    public static float MasterVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
    public static bool Fullscreen => PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

    public static void BeginNewRun()
    {
        DeleteRunSaveFiles();
        DeleteLegacyRunSave();
    }

    public static void AbandonRun()
    {
        DeleteRunSaveFiles();
        DeleteLegacyRunSave();
    }

    public static void SaveRun(RunSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        saveData.version = RunSaveData.CurrentVersion;
        string json = JsonUtility.ToJson(saveData, true);
        try
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            File.WriteAllText(RunSaveTemporaryPath, json, new UTF8Encoding(false));

            if (File.Exists(RunSavePath))
            {
                ReplaceRunSaveFile();
            }
            else
            {
                File.Move(RunSaveTemporaryPath, RunSavePath);
            }

            DeleteLegacyRunSave();
        }
        catch (Exception exception)
        {
            Debug.LogError($"Run save could not be written to '{RunSavePath}': {exception.Message}");
        }
    }

    public static bool TryLoadRun(out RunSaveData saveData)
    {
        if (TryReadSaveFile(RunSavePath, out saveData))
        {
            return true;
        }

        if (TryReadSaveFile(RunSaveBackupPath, out saveData)
            || TryReadSaveFile(RunSaveTemporaryPath, out saveData))
        {
            TryDeleteFile(RunSavePath);
            SaveRun(saveData);
            return true;
        }

        if (TryReadLegacySave(out saveData))
        {
            SaveRun(saveData);
            return true;
        }

        bool hadSaveData = File.Exists(RunSavePath)
            || File.Exists(RunSaveBackupPath)
            || File.Exists(RunSaveTemporaryPath)
            || PlayerPrefs.HasKey(LegacyRunDataKey);
        if (hadSaveData)
        {
            Debug.LogWarning("No valid run save could be read. Corrupted or unsupported save data was cleared.");
            AbandonRun();
        }

        saveData = null;
        return false;
    }

    private static bool HasValidFileSave()
    {
        return TryReadSaveFile(RunSavePath, out _)
            || TryReadSaveFile(RunSaveBackupPath, out _)
            || TryReadSaveFile(RunSaveTemporaryPath, out _);
    }

    private static bool TryReadSaveFile(string path, out RunSaveData saveData)
    {
        saveData = null;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return false;
        }

        try
        {
            return TryParseSaveJson(File.ReadAllText(path, Encoding.UTF8), out saveData);
        }
        catch (Exception)
        {
            saveData = null;
            return false;
        }
    }

    private static bool TryReadLegacySave(out RunSaveData saveData)
    {
        saveData = null;
        if (PlayerPrefs.GetInt(LegacyActiveRunKey, 0) != 1
            || !PlayerPrefs.HasKey(LegacyRunDataKey))
        {
            return false;
        }

        return TryParseSaveJson(PlayerPrefs.GetString(LegacyRunDataKey, string.Empty), out saveData);
    }

    private static bool TryParseSaveJson(string json, out RunSaveData saveData)
    {
        saveData = null;
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            saveData = JsonUtility.FromJson<RunSaveData>(json);
            return saveData != null && saveData.version == RunSaveData.CurrentVersion;
        }
        catch (Exception)
        {
            saveData = null;
            return false;
        }
    }

    private static void ReplaceRunSaveFile()
    {
        try
        {
            File.Replace(RunSaveTemporaryPath, RunSavePath, RunSaveBackupPath, true);
        }
        catch (PlatformNotSupportedException)
        {
            CopyRunSaveFileFallback();
        }
        catch (IOException)
        {
            CopyRunSaveFileFallback();
        }
    }

    private static void CopyRunSaveFileFallback()
    {
        File.Copy(RunSavePath, RunSaveBackupPath, true);
        File.Copy(RunSaveTemporaryPath, RunSavePath, true);
        TryDeleteFile(RunSaveTemporaryPath);
    }

    private static void DeleteRunSaveFiles()
    {
        TryDeleteFile(RunSavePath);
        TryDeleteFile(RunSaveBackupPath);
        TryDeleteFile(RunSaveTemporaryPath);
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Run save file could not be deleted at '{path}': {exception.Message}");
        }
    }

    private static void DeleteLegacyRunSave()
    {
        bool changed = false;
        if (PlayerPrefs.HasKey(LegacyActiveRunKey))
        {
            PlayerPrefs.DeleteKey(LegacyActiveRunKey);
            changed = true;
        }

        if (PlayerPrefs.HasKey(LegacyRunDataKey))
        {
            PlayerPrefs.DeleteKey(LegacyRunDataKey);
            changed = true;
        }

        if (changed)
        {
            PlayerPrefs.Save();
        }
    }

    public static void SetMasterVolume(float value)
    {
        float clampedValue = Mathf.Clamp01(value);
        AudioListener.volume = clampedValue;
        PlayerPrefs.SetFloat(MasterVolumeKey, clampedValue);
        PlayerPrefs.Save();
    }

    public static void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt(FullscreenKey, fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ApplySettings()
    {
        AudioListener.volume = MasterVolume;
        Screen.fullScreen = Fullscreen;
    }
}
