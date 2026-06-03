using UnityEngine;

public sealed class WeaponBag : ShopBagBase
{
    [SerializeField, Min(1)] private int maxWeapons = 6;
    [SerializeField] private Sprite fallbackWeaponIcon;

    public int MaxWeapons => maxWeapons;
    public bool IsFull => Count >= maxWeapons;

    protected override string MissingBagMessage => "武器背包没有找到 Content。";

    protected override bool CanAdd(ShopContentDefinition content, out string failureReason)
    {
        failureReason = string.Empty;
        if (content.Kind != ShopContentKind.Weapon)
        {
            failureReason = "这个商品不是武器，不能放入武器背包。";
            return false;
        }

        if (IsFull)
        {
            failureReason = $"武器背包已满（{Count}/{maxWeapons}）。";
            return false;
        }

        return true;
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
