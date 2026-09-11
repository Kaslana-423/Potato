using System;
using UnityEngine;

public sealed class WeaponBag : ShopBagBase, IPlayerWeaponLoadout
{
    [SerializeField, Min(1)] private int maxWeapons = 6;
    [SerializeField] private Sprite fallbackWeaponIcon;

    [Header("Starting Weapon")]
    [SerializeField] private bool addStartingWeapon = true;
    [SerializeField] private string startingWeaponId = "weapon.stick.tier_1";

    public int MaxWeapons => maxWeapons;
    public bool IsFull => Count >= maxWeapons;
    public int WeaponCount => Count;

    event Action IPlayerWeaponLoadout.Changed
    {
        add => ContentsChanged += value;
        remove => ContentsChanged -= value;
    }

    protected override string MissingBagMessage => "武器背包没有找到 Content。";

    protected override void Awake()
    {
        base.Awake();
        EnsureStartingWeapon();
    }

    public ShopWeaponDefinition GetWeapon(int index)
    {
        return index >= 0 && index < Count
            ? Contents[index] as ShopWeaponDefinition
            : null;
    }

    public void EnsureStartingWeapon()
    {
        string resolvedStartingWeaponId = ResolveStartingWeaponId();
        if (!addStartingWeapon || Count > 0 || string.IsNullOrWhiteSpace(resolvedStartingWeaponId))
        {
            return;
        }

        ShopWeaponDefinition startingWeapon = FindStartingWeapon();
        if (startingWeapon != null)
        {
            if (!TryAdd(startingWeapon, out string failureReason))
            {
                Debug.LogWarning($"Could not add starting weapon '{resolvedStartingWeaponId}': {failureReason}", this);
            }

            return;
        }

        Debug.LogWarning($"Starting weapon '{resolvedStartingWeaponId}' was not found in the shop catalog.", this);
    }

    private string ResolveStartingWeaponId()
    {
        if (!string.IsNullOrWhiteSpace(GameSessionState.CurrentStartingWeaponId))
        {
            return GameSessionState.CurrentStartingWeaponId;
        }

        CharacterDefinition character = CharacterCatalog.FindById(GameSessionState.CurrentCharacterId);
        return character != null && !string.IsNullOrWhiteSpace(character.StartingWeaponId)
            ? character.StartingWeaponId
            : startingWeaponId;
    }

    protected override void NormalizeRestoredContents()
    {
        if (!addStartingWeapon || Count > 0)
        {
            return;
        }

        ShopWeaponDefinition startingWeapon = FindStartingWeapon();
        if (startingWeapon != null)
        {
            MutableContents.Add(startingWeapon);
        }
    }

    private ShopWeaponDefinition FindStartingWeapon()
    {
        string resolvedStartingWeaponId = ResolveStartingWeaponId();
        if (string.IsNullOrWhiteSpace(resolvedStartingWeaponId))
        {
            return null;
        }

        foreach (ShopContentDefinition content in ShopContentCatalog.All)
        {
            if (content is ShopWeaponDefinition weapon
                && string.Equals(content.Id, resolvedStartingWeaponId, StringComparison.OrdinalIgnoreCase))
            {
                return weapon;
            }
        }

        return null;
    }

    protected override bool CanAdd(ShopContentDefinition content, out string failureReason)
    {
        failureReason = string.Empty;
        if (content.Kind != ShopContentKind.Weapon)
        {
            failureReason = "这件商品不是武器，无法放入武器背包。";
            return false;
        }

        if (IsFull)
        {
            failureReason = $"武器背包已满（{Count}/{maxWeapons}），请先合成或回收武器。";
            return false;
        }

        return true;
    }

    public bool CanCombineAt(int selectedIndex, out string failureReason)
    {
        failureReason = string.Empty;
        ShopWeaponDefinition selectedWeapon = GetWeapon(selectedIndex);
        if (selectedWeapon == null)
        {
            failureReason = "没有选中可合成的武器。";
            return false;
        }

        if (!TryFindUpgrade(selectedWeapon, out _))
        {
            failureReason = "该武器已经达到最高品质。";
            return false;
        }

        if (!TryFindMatchingWeaponIndex(selectedWeapon, selectedIndex, out _))
        {
            failureReason = "需要另一把同类型、同品质的武器。";
            return false;
        }

        return true;
    }

    public bool TryCombineAt(
        int selectedIndex,
        out ShopWeaponDefinition upgradedWeapon,
        out int upgradedIndex,
        out string failureReason)
    {
        upgradedWeapon = null;
        upgradedIndex = -1;
        if (!CanCombineAt(selectedIndex, out failureReason))
        {
            return false;
        }

        ShopWeaponDefinition selectedWeapon = GetWeapon(selectedIndex);
        TryFindMatchingWeaponIndex(selectedWeapon, selectedIndex, out int matchingIndex);
        TryFindUpgrade(selectedWeapon, out upgradedWeapon);

        int firstIndex = Mathf.Min(selectedIndex, matchingIndex);
        int secondIndex = Mathf.Max(selectedIndex, matchingIndex);
        MutableContents[firstIndex] = upgradedWeapon;
        MutableContents.RemoveAt(secondIndex);
        upgradedIndex = firstIndex;
        CommitContentsChange();
        return true;
    }

    public bool TryRecycleAt(
        int selectedIndex,
        out ShopWeaponDefinition recycledWeapon,
        out int refund,
        out string failureReason)
    {
        recycledWeapon = GetWeapon(selectedIndex);
        refund = 0;
        failureReason = string.Empty;
        if (recycledWeapon == null)
        {
            failureReason = "没有选中可回收的武器。";
            return false;
        }

        refund = CalculateRecycleValue(recycledWeapon);
        MutableContents.RemoveAt(selectedIndex);
        CommitContentsChange();
        return true;
    }

    public static int CalculateRecycleValue(ShopWeaponDefinition weapon)
    {
        return weapon == null ? 0 : Mathf.Max(1, Mathf.CeilToInt(weapon.BasePrice * 0.5f));
    }

    private bool TryFindMatchingWeaponIndex(
        ShopWeaponDefinition weapon,
        int excludedIndex,
        out int matchingIndex)
    {
        for (int index = 0; index < MutableContents.Count; index++)
        {
            if (index == excludedIndex)
            {
                continue;
            }

            ShopWeaponDefinition existingWeapon = MutableContents[index] as ShopWeaponDefinition;
            if (existingWeapon != null
                && existingWeapon.Rarity == weapon.Rarity
                && string.Equals(existingWeapon.FamilyId, weapon.FamilyId, StringComparison.OrdinalIgnoreCase))
            {
                matchingIndex = index;
                return true;
            }
        }

        matchingIndex = -1;
        return false;
    }

    private static bool TryFindUpgrade(
        ShopWeaponDefinition weapon,
        out ShopWeaponDefinition upgradedWeapon)
    {
        upgradedWeapon = null;
        if (weapon == null || weapon.Rarity >= ShopRarity.Tier4)
        {
            return false;
        }

        ShopRarity targetRarity = (ShopRarity)((int)weapon.Rarity + 1);
        foreach (ShopContentDefinition content in ShopContentCatalog.All)
        {
            ShopWeaponDefinition candidate = content as ShopWeaponDefinition;
            if (candidate != null
                && candidate.Rarity == targetRarity
                && string.Equals(candidate.FamilyId, weapon.FamilyId, StringComparison.OrdinalIgnoreCase))
            {
                upgradedWeapon = candidate;
                return true;
            }
        }

        return false;
    }

    protected override Sprite GetFallbackIcon(ShopContentDefinition content)
    {
        if (fallbackWeaponIcon == null)
        {
            fallbackWeaponIcon = Resources.Load<Sprite>("Weapon/weapon");
        }

        return fallbackWeaponIcon;
    }
}
