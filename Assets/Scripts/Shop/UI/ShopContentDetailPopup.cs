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
    private GameObject previousSelection;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public void Show(ShopContentDefinition content)
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }
}
