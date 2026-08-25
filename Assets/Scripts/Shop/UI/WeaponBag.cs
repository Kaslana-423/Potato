using System;
using UnityEngine;

public sealed class WeaponBag : ShopBagBase
{
    [SerializeField, Min(1)] private int maxWeapons = 6;
    [SerializeField] private Sprite fallbackWeaponIcon;

    [Header("Starting Weapon")]
    [SerializeField] private bool addStartingWeapon = true;
    [SerializeField] private string startingWeaponId = "weapon.stick.tier_1";

    public int MaxWeapons => maxWeapons;
    public bool IsFull => Count >= maxWeapons;
    public ShopWeaponDefinition LastAddedWeapon { get; private set; }
    public int LastCombinationCount { get; private set; }
    public bool LastAddCombined => LastCombinationCount > 0;

    protected override string MissingBagMessage => "武器背包没有找到 Content。";

    private void Start()
    {
        EnsureStartingWeapon();
    }

    public void EnsureStartingWeapon()
    {
        if (!addStartingWeapon || Count > 0 || string.IsNullOrWhiteSpace(startingWeaponId))
        {
            return;
        }

        foreach (ShopContentDefinition content in ShopContentCatalog.All)
        {
            if (content is ShopWeaponDefinition
                && string.Equals(content.Id, startingWeaponId, StringComparison.OrdinalIgnoreCase))
            {
                if (!TryAdd(content, out string failureReason))
                {
                    Debug.LogWarning($"Could not add starting weapon '{startingWeaponId}': {failureReason}", this);
                }

                return;
            }
        }

        Debug.LogWarning($"Starting weapon '{startingWeaponId}' was not found in the shop catalog.", this);
    }

    protected override bool CanAdd(ShopContentDefinition content, out string failureReason)
    {
        failureReason = string.Empty;
        if (content.Kind != ShopContentKind.Weapon)
        {
            failureReason = "这个商品不是武器，不能放入武器背包。";
            return false;
        }

        ShopWeaponDefinition weapon = content as ShopWeaponDefinition;
        if (IsFull && (weapon == null || !CanCombine(weapon)))
        {
            failureReason = $"武器背包已满（{Count}/{maxWeapons}）。";
            return false;
        }

        return true;
    }

    protected override void StoreContent(ShopContentDefinition content)
    {
        ShopWeaponDefinition pendingWeapon = content as ShopWeaponDefinition;
        LastAddedWeapon = pendingWeapon;
        LastCombinationCount = 0;
        if (pendingWeapon == null)
        {
            base.StoreContent(content);
            return;
        }

        while (TryFindMatchingWeaponIndex(pendingWeapon, out int matchingIndex)
            && TryFindUpgrade(pendingWeapon, out ShopWeaponDefinition upgradedWeapon))
        {
            MutableContents.RemoveAt(matchingIndex);
            pendingWeapon = upgradedWeapon;
            LastCombinationCount++;
        }

        LastAddedWeapon = pendingWeapon;
        if (!LastAddCombined)
        {
            base.StoreContent(content);
            return;
        }

        MutableContents.Add(pendingWeapon);
        RebuildSlotViews();
    }

    private bool CanCombine(ShopWeaponDefinition weapon)
    {
        return TryFindMatchingWeaponIndex(weapon, out _)
            && TryFindUpgrade(weapon, out _);
    }

    private bool TryFindMatchingWeaponIndex(ShopWeaponDefinition weapon, out int matchingIndex)
    {
        for (int index = 0; index < MutableContents.Count; index++)
        {
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
