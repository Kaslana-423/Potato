using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class ShopContentDetailPopup : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text metaText;
    [SerializeField] private TMP_Text detailsText;
    [SerializeField] private Button closeButton;

    [Header("Weapon Actions")]
    [SerializeField] private Button combineButton;
    [SerializeField] private Button recycleButton;
    [SerializeField] private TMP_Text recycleValueText;

    private GameObject previousSelection;
    private WeaponBag selectedWeaponBag;
    private int selectedWeaponIndex = -1;

    private void Awake()
    {
        AutoBindWeaponActions();
        closeButton?.onClick.AddListener(Hide);
        combineButton?.onClick.AddListener(CombineSelectedWeapon);
        recycleButton?.onClick.AddListener(RecycleSelectedWeapon);
    }

    private void OnDestroy()
    {
        closeButton?.onClick.RemoveListener(Hide);
        combineButton?.onClick.RemoveListener(CombineSelectedWeapon);
        recycleButton?.onClick.RemoveListener(RecycleSelectedWeapon);
    }

    public void Show(ShopContentDefinition content)
    {
        Show(content, null, -1);
    }

    public void Show(ShopContentDefinition content, ShopBagBase sourceBag, int contentIndex)
    {
        if (content == null)
        {
            return;
        }

        icon.sprite = content.LoadIcon();
        icon.color = icon.sprite != null ? Color.white : Color.clear;
        titleText.text = content.LocalizedDisplayName;
        string kind = ShopLocalization.GetKindLabel(content.Kind);
        if (content is ShopWeaponDefinition weapon && !string.IsNullOrWhiteSpace(weapon.LocalizedClassTags))
        {
            kind += $" · {weapon.LocalizedClassTags}";
        }

        metaText.text = $"{kind} · {content.RarityLabel}";
        string details = content.BuildDetails();
        detailsText.text = string.IsNullOrWhiteSpace(details) ? "暂无详细说明" : details;
        ConfigureWeaponActions(content, sourceBag as WeaponBag, contentIndex);
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != closeButton.gameObject)
        {
            previousSelection = EventSystem.current.currentSelectedGameObject;
        }

        gameObject.SetActive(true);
        closeButton.Select();
    }

    public void Hide()
    {
        bool restoreSelection = EventSystem.current != null
            && EventSystem.current.currentSelectedGameObject == closeButton.gameObject;
        gameObject.SetActive(false);
        if (restoreSelection && previousSelection != null && previousSelection.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(previousSelection);
        }

        previousSelection = null;
        selectedWeaponBag = null;
        selectedWeaponIndex = -1;
        HideWeaponActions();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    private void ConfigureWeaponActions(
        ShopContentDefinition content,
        WeaponBag weaponBag,
        int contentIndex)
    {
        ShopWeaponDefinition weapon = content as ShopWeaponDefinition;
        selectedWeaponBag = weapon != null ? weaponBag : null;
        selectedWeaponIndex = selectedWeaponBag != null ? contentIndex : -1;
        bool canManage = selectedWeaponBag != null && selectedWeaponBag.GetWeapon(contentIndex) == weapon;

        if (combineButton != null)
        {
            combineButton.gameObject.SetActive(canManage);
            combineButton.interactable = canManage
                && selectedWeaponBag.CanCombineAt(contentIndex, out _);
        }

        if (recycleButton != null)
        {
            recycleButton.gameObject.SetActive(canManage);
            recycleButton.interactable = canManage;
        }

        if (recycleValueText != null)
        {
            recycleValueText.text = canManage
                ? WeaponBag.CalculateRecycleValue(weapon).ToString()
                : string.Empty;
        }
    }

    private void CombineSelectedWeapon()
    {
        WeaponBag weaponBag = selectedWeaponBag;
        int weaponIndex = selectedWeaponIndex;
        if (weaponBag == null
            || !weaponBag.TryCombineAt(
                weaponIndex,
                out ShopWeaponDefinition upgradedWeapon,
                out int upgradedIndex,
                out _))
        {
            return;
        }

        Show(upgradedWeapon, weaponBag, upgradedIndex);
    }

    private void RecycleSelectedWeapon()
    {
        WeaponBag weaponBag = selectedWeaponBag;
        int weaponIndex = selectedWeaponIndex;
        if (weaponBag == null
            || !weaponBag.TryRecycleAt(weaponIndex, out _, out int refund, out _))
        {
            return;
        }

        PlayerWallet.GetOrCreate().AddCoins(refund);
        Hide();
    }

    private void AutoBindWeaponActions()
    {
        combineButton = combineButton != null ? combineButton : FindButton("Compose Button");
        recycleButton = recycleButton != null ? recycleButton : FindButton("Sell Button", "Recycle Button");
        if (recycleValueText == null && recycleButton != null)
        {
            foreach (TMP_Text text in recycleButton.GetComponentsInChildren<TMP_Text>(true))
            {
                if (text.name == "CoinNum")
                {
                    recycleValueText = text;
                    break;
                }
            }
        }
    }

    private Button FindButton(params string[] names)
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            foreach (string objectName in names)
            {
                if (button.name == objectName)
                {
                    return button;
                }
            }
        }

        return null;
    }

    private void HideWeaponActions()
    {
        combineButton?.gameObject.SetActive(false);
        recycleButton?.gameObject.SetActive(false);
    }
}
