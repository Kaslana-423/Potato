using UnityEngine;

public static class GameSessionState
{
    private const string ActiveRunKey = "potato.session.active";
    private const string MasterVolumeKey = "potato.settings.master_volume";
    private const string FullscreenKey = "potato.settings.fullscreen";

    public static bool HasActiveRun => PlayerPrefs.GetInt(ActiveRunKey, 0) == 1;
    public static float MasterVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
    public static bool Fullscreen => PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

    public static void BeginNewRun()
    {
        PlayerPrefs.SetInt(ActiveRunKey, 1);
        PlayerPrefs.Save();
    }

    public static void AbandonRun()
    {
        PlayerPrefs.DeleteKey(ActiveRunKey);
        PlayerPrefs.Save();
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
