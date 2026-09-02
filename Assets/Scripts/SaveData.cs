using System;

[Serializable]
public sealed class SaveData
{
    public const int CurrentVersion = 1;

    public int version = CurrentVersion;
    public int slotId;
    public string saveId;
    public string displayName;
    public string createdAtUtc;
    public string lastPlayedAtUtc;
}

public readonly struct SaveSlotInfo
{
    public SaveSlotInfo(int slotId, bool exists, bool isValid, string displayName, string lastPlayedAtUtc)
    {
        SlotId = slotId;
        Exists = exists;
        IsValid = isValid;
        DisplayName = displayName;
        LastPlayedAtUtc = lastPlayedAtUtc;
    }

    public int SlotId { get; }
    public bool Exists { get; }
    public bool IsValid { get; }
    public string DisplayName { get; }
    public string LastPlayedAtUtc { get; }
}
