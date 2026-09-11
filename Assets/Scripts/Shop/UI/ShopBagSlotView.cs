using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class ShopBagSlotView : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text placeholder;
    private ShopContentDefinition content;
    private ShopContentDetailPopup detailPopup;
    private ShopBagBase sourceBag;
    private int contentIndex = -1;
    public bool HasSceneReferences => icon != null && placeholder != null;

    public void Bind(
        ShopContentDefinition value,
        Sprite fallback,
        ShopContentDetailPopup popup,
        ShopBagBase bag,
        int index)
    {
        content = value;
        detailPopup = popup;
        sourceBag = bag;
        contentIndex = index;
        gameObject.SetActive(content != null);
        if (content == null)
        {
            return;
        }

        Sprite sprite = content.LoadIcon();
        icon.sprite = sprite != null ? sprite : fallback;
        icon.color = icon.sprite != null ? Color.white : Color.clear;
        placeholder.gameObject.SetActive(icon.sprite == null);
        placeholder.text = string.IsNullOrWhiteSpace(content.LocalizedDisplayName)
            ? "?" : content.LocalizedDisplayName.Substring(0, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left
            || eventData.button == PointerEventData.InputButton.Right)
        {
            detailPopup?.Show(content, sourceBag, contentIndex);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        detailPopup?.Show(content, sourceBag, contentIndex);
    }
}
