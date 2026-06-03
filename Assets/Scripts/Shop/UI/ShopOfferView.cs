using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopOfferView : MonoBehaviour
{
    [Header("Card References")]
    [SerializeField] private GameObject IconPanel;
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text iconPlaceholder;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text kindText;
    [SerializeField] private TMP_Text limitText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button inspectButton;
    [SerializeField] private Button buyButton;

    private ShopContentDefinition content;
    private Action<ShopContentDefinition> inspectAction;
    private Action<ShopOfferView, ShopContentDefinition> buyAction;

    private void Awake()
    {
        AutoBindReferences();
        BindInspectButton();
        BindBuyButton();
    }

    private void Reset()
    {
        AutoBindReferences();
    }

    private void OnValidate()
    {
        AutoBindReferences();
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        if (background == null)
        {
            background = GetComponent<Image>();
        }

        if (background == null)
        {
            background = FindComponent<Image>("Background");
        }

        if (icon == null)
        {
            icon = FindComponent<Image>("Icon");
        }

        if (iconPlaceholder == null)
        {
            iconPlaceholder = FindComponent<TMP_Text>("IconPlaceholder", "Icon Placeholder");
        }

        if (nameText == null)
        {
            nameText = FindComponent<TMP_Text>("NameText", "Name");
        }

        if (kindText == null)
        {
            kindText = FindComponent<TMP_Text>("KindText", "Kind");
        }

        if (limitText == null)
        {
            limitText = FindComponent<TMP_Text>("LimitText", "Limit");
        }

        if (descriptionText == null)
        {
            descriptionText = FindComponent<TMP_Text>("DescriptionText", "Description");
        }

        if (statsText == null)
        {
            statsText = FindComponent<TMP_Text>("StatsText", "Stats");
        }

        if (priceText == null)
        {
            priceText = FindComponent<TMP_Text>("PriceText", "Price");
        }

        if (inspectButton == null)
        {
            inspectButton = GetComponent<Button>();
        }

        if (inspectButton == null)
        {
            inspectButton = FindComponent<Button>("InspectButton");
        }

        if (buyButton == null)
        {
            buyButton = FindComponent<Button>("PricePanel", "BuyButton", "PriceButton");
        }
    }

    public void Configure(
        Image newBackground,
        Image newIcon,
        TMP_Text newIconPlaceholder,
        TMP_Text newNameText,
        TMP_Text newKindText,
        TMP_Text newDescriptionText,
        TMP_Text newStatsText,
        TMP_Text newPriceText,
        Button newInspectButton,
        TMP_Text newLimitText = null,
        Button newBuyButton = null)
    {
        background = newBackground;
        icon = newIcon;
        iconPlaceholder = newIconPlaceholder;
        nameText = newNameText;
        kindText = newKindText;
        descriptionText = newDescriptionText;
        statsText = newStatsText;
        priceText = newPriceText;
        inspectButton = newInspectButton;
        limitText = newLimitText;
        buyButton = newBuyButton;

        BindInspectButton();
        BindBuyButton();
    }

    private void BindInspectButton()
    {
        if (inspectButton != null)
        {
            if (inspectButton.targetGraphic == null && background != null)
            {
                inspectButton.targetGraphic = background;
            }

            inspectButton.onClick.RemoveListener(Inspect);
            inspectButton.onClick.AddListener(Inspect);
        }
    }

    private void BindBuyButton()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(Buy);
            buyButton.onClick.AddListener(Buy);
        }
    }

    public void Bind(
        ShopContentDefinition newContent,
        Action<ShopContentDefinition> newInspectAction,
        Action<ShopOfferView, ShopContentDefinition> newBuyAction = null)
    {
        content = newContent;
        inspectAction = newInspectAction;
        buyAction = newBuyAction;

        if (content == null)
        {
            return;
        }

        SetCardContentVisible(true);
        if (inspectButton != null)
        {
            inspectButton.interactable = true;
        }

        if (buyButton != null)
        {
            buyButton.interactable = true;
        }

        Sprite loadedIcon = content.LoadIcon();
        if (icon != null)
        {
            icon.sprite = loadedIcon;
            icon.color = loadedIcon != null ? Color.white : GetPlaceholderColor(content.Kind);
        }

        if (background != null)
        {
            background.color = GetRarityColor(content.Rarity);
        }

        if (iconPlaceholder != null)
        {
            iconPlaceholder.gameObject.SetActive(loadedIcon == null);
            iconPlaceholder.text = content.Kind == ShopContentKind.Weapon ? "武" : "道";
        }

        if (nameText != null)
        {
            nameText.text = content.LocalizedDisplayName;
        }

        if (kindText != null)
        {
            kindText.text = BuildKindLabel(content);
        }

        if (descriptionText != null)
        {
            descriptionText.text = string.IsNullOrWhiteSpace(content.LocalizedDescription)
                ? string.Empty
                : $"• {content.LocalizedDescription}";
            descriptionText.gameObject.SetActive(!string.IsNullOrWhiteSpace(descriptionText.text));
        }

        if (statsText != null)
        {
            statsText.text = content.BuildStatLine();
        }

        if (priceText != null)
        {
            priceText.text = $"<color=#90E65A>{content.BasePrice}</color> 材料";
        }

        BindLimit(content);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void MarkPurchased()
    {
        content = null;
        inspectAction = null;
        buyAction = null;

        SetCardContentVisible(false);
        if (background != null)
        {
            background.color = new Color(0f, 0f, 0f, 0f);
        }

        if (inspectButton != null)
        {
            inspectButton.interactable = false;
        }

        if (buyButton != null)
        {
            buyButton.interactable = false;
        }
    }

    private void Inspect()
    {
        inspectAction?.Invoke(content);
    }

    private void Buy()
    {
        buyAction?.Invoke(this, content);
    }

    private void SetCardContentVisible(bool visible)
    {
        SetGameObjectActive(icon, visible);
        SetGameObjectActive(iconPlaceholder, visible);
        SetGameObjectActive(nameText, visible);
        SetGameObjectActive(kindText, visible);
        SetGameObjectActive(limitText, visible);
        SetGameObjectActive(descriptionText, visible);
        SetGameObjectActive(statsText, visible);
        SetGameObjectActive(priceText, visible);
        IconPanel.SetActive(visible);
        if (buyButton != null)
        {
            buyButton.gameObject.SetActive(visible);
        }
    }

    private static void SetGameObjectActive(Component component, bool active)
    {
        if (component != null)
        {
            component.gameObject.SetActive(active);
        }
    }

    private void BindLimit(ShopContentDefinition newContent)
    {
        if (limitText == null)
        {
            return;
        }

        ShopItemDefinition item = newContent as ShopItemDefinition;
        bool hasLimit = item != null && item.PurchaseLimit > 0;
        limitText.gameObject.SetActive(hasLimit);
        if (hasLimit)
        {
            limitText.text = $"限制 (0/{item.PurchaseLimit})";
        }
    }

    private static string BuildKindLabel(ShopContentDefinition newContent)
    {
        ShopWeaponDefinition weapon = newContent as ShopWeaponDefinition;
        if (weapon == null || string.IsNullOrWhiteSpace(weapon.LocalizedClassTags))
        {
            return ShopLocalization.GetKindLabel(newContent.Kind);
        }

        return $"{ShopLocalization.GetKindLabel(newContent.Kind)} · {weapon.LocalizedClassTags}";
    }

    private static Color GetPlaceholderColor(ShopContentKind kind)
    {
        return kind == ShopContentKind.Weapon
            ? new Color(0.32f, 0.20f, 0.16f, 1f)
            : new Color(0.15f, 0.30f, 0.22f, 1f);
    }

    private static Color GetRarityColor(ShopRarity rarity)
    {
        switch (rarity)
        {
            case ShopRarity.Tier2:
                return new Color(0.055f, 0.13f, 0.085f, 0.98f);
            case ShopRarity.Tier3:
                return new Color(0.06f, 0.085f, 0.16f, 0.98f);
            case ShopRarity.Tier4:
                return new Color(0.15f, 0.065f, 0.18f, 0.98f);
            default:
                return new Color(0.035f, 0.035f, 0.035f, 0.98f);
        }
    }

    private Transform FindDescendant(params string[] names)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (child.name == objectName)
                {
                    return child;
                }
            }
        }

        return null;
    }

    private T FindComponent<T>(params string[] names) where T : Component
    {
        Transform child = FindDescendant(names);
        return child != null ? child.GetComponent<T>() : null;
    }
}
