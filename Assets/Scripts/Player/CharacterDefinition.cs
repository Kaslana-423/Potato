using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class CharacterStatModifier
{
    [SerializeField] private PlayerStatId statId;
    [SerializeField] private int amount;

    public PlayerStatId StatId => statId;
    public int Amount => amount;
}

[CreateAssetMenu(fileName = "Character", menuName = "Potato/Character Definition")]
public sealed class CharacterDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id = "character.new";
    [SerializeField, Min(0)] private int displayOrder;
    [SerializeField] private bool visibleInSelection = true;
    [SerializeField] private bool unlocked = true;

    [Header("Presentation")]
    [SerializeField] private string displayName = "新角色";
    [SerializeField] private string typeLabel = "待配置";
    [SerializeField, TextArea(2, 5)] private string description = "请配置角色说明。";
    [SerializeField] private Sprite portrait;

    [Header("Starting Loadout")]
    [SerializeField] private string startingWeaponId = "weapon.stick.tier_1";
    [SerializeField] private string startingWeaponDisplayName = "木棍";
    [SerializeField] private List<CharacterStatModifier> startingStatModifiers = new List<CharacterStatModifier>();

    public string Id => id;
    public int DisplayOrder => displayOrder;
    public bool VisibleInSelection => visibleInSelection;
    public bool Unlocked => unlocked;
    public string DisplayName => displayName;
    public string TypeLabel => typeLabel;
    public string Description => description;
    public Sprite Portrait => portrait;
    public string StartingWeaponId => startingWeaponId;
    public string StartingWeaponDisplayName => startingWeaponDisplayName;
    public IReadOnlyList<CharacterStatModifier> StartingStatModifiers => startingStatModifiers;

    private void OnValidate()
    {
        displayOrder = Mathf.Max(0, displayOrder);
        id = id?.Trim();
        startingWeaponId = startingWeaponId?.Trim();
    }
}
