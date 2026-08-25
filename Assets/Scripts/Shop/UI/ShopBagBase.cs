using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ShopBagBase : MonoBehaviour
{
    private const string ChineseFontResourcePath = "Fonts & Materials/SmileySans-Oblique SDF";

    [Header("Bag References")]
    [SerializeField] private Transform contentRoot;

    [Header("Slot Visuals")]
    [SerializeField] private Vector2 slotSize = new Vector2(120f, 120f);
    [SerializeField] private Vector2 iconSize = new Vector2(96f, 96f);
    [SerializeField] private Color slotBackgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.92f);
    [SerializeField] private Color placeholderColor = new Color(0.92f, 0.92f, 0.92f, 1f);

    private readonly List<ShopContentDefinition> contents = new List<ShopContentDefinition>();
    private TMP_FontAsset cachedFont;
    private ShopContentDetailPopup detailPopup;

    public IReadOnlyList<ShopContentDefinition> Contents => contents;
    public int Count => contents.Count;
    protected List<ShopContentDefinition> MutableContents => contents;
    public event Action ContentsChanged;

    protected virtual string MissingBagMessage => "背包未设置。";

    private void Awake()
    {
        AutoBindReferences();
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
        if (contentRoot == null)
        {
            contentRoot = FindDescendant("Content");
        }

        if (contentRoot == null)
        {
            GridLayoutGroup grid = GetComponentInChildren<GridLayoutGroup>(true);
            if (grid != null)
            {
                contentRoot = grid.transform;
            }
        }
    }

    public bool TryAdd(ShopContentDefinition content, out string failureReason)
    {
        failureReason = string.Empty;
        AutoBindReferences();

        if (content == null)
        {
            failureReason = "商品为空，无法加入背包。";
            return false;
        }

        if (contentRoot == null)
        {
            failureReason = MissingBagMessage;
            return false;
        }

        if (!CanAdd(content, out failureReason))
        {
            return false;
        }

        StoreContent(content);
        ContentsChanged?.Invoke();
        return true;
    }

    public bool CanAccept(ShopContentDefinition content, out string failureReason)
    {
        failureReason = string.Empty;
        AutoBindReferences();

        if (content == null)
        {
            failureReason = "商品为空，无法加入背包。";
            return false;
        }

        if (contentRoot == null)
        {
            failureReason = MissingBagMessage;
            return false;
        }

        return CanAdd(content, out failureReason);
    }

    [ContextMenu("Clear Bag")]
    public void Clear()
    {
        contents.Clear();
        RebuildSlotViews();
        ContentsChanged?.Invoke();
    }

    protected virtual void StoreContent(ShopContentDefinition content)
    {
        detailPopup?.Hide();
        contents.Add(content);
        CreateSlot(content);
    }

    protected void RebuildSlotViews()
    {
        detailPopup?.Hide();
        AutoBindReferences();
        if (contentRoot == null)
        {
            return;
        }

        for (int index = contentRoot.childCount - 1; index >= 0; index--)
        {
            Transform child = contentRoot.GetChild(index);
            child.gameObject.SetActive(false);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        foreach (ShopContentDefinition content in contents)
        {
            CreateSlot(content);
        }
    }

    protected abstract bool CanAdd(ShopContentDefinition content, out string failureReason);

    protected virtual Sprite GetFallbackIcon(ShopContentDefinition content)
    {
        return null;
    }

    private void CreateSlot(ShopContentDefinition content)
    {
        GameObject slot = new GameObject($"BagSlot - {content.LocalizedDisplayName}", typeof(RectTransform));
        slot.layer = 5;
        slot.transform.SetParent(contentRoot, false);

        RectTransform slotRect = slot.GetComponent<RectTransform>();
        slotRect.sizeDelta = slotSize;

        LayoutElement layoutElement = slot.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = slotSize.x;
        layoutElement.preferredHeight = slotSize.y;

        Image background = slot.AddComponent<Image>();
        background.color = slotBackgroundColor;
        background.raycastTarget = true;

        BagSlotRightClickHandler rightClickHandler = slot.AddComponent<BagSlotRightClickHandler>();
        rightClickHandler.Configure(screenPosition => ShowDetails(content, screenPosition));

        GameObject iconObject = new GameObject("Icon", typeof(RectTransform));
        iconObject.layer = 5;
        iconObject.transform.SetParent(slot.transform, false);

        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = iconSize;

        Image icon = iconObject.AddComponent<Image>();
        Sprite sprite = content.LoadIcon();
        if (sprite == null)
        {
            sprite = GetFallbackIcon(content);
        }

        icon.sprite = sprite;
        icon.color = sprite != null ? Color.white : Color.clear;
        icon.preserveAspect = true;

        GameObject placeholderObject = new GameObject("Placeholder", typeof(RectTransform));
        placeholderObject.layer = 5;
        placeholderObject.transform.SetParent(slot.transform, false);

        RectTransform placeholderRect = placeholderObject.GetComponent<RectTransform>();
        placeholderRect.anchorMin = new Vector2(0.5f, 0.5f);
        placeholderRect.anchorMax = new Vector2(0.5f, 0.5f);
        placeholderRect.pivot = new Vector2(0.5f, 0.5f);
        placeholderRect.anchoredPosition = Vector2.zero;
        placeholderRect.sizeDelta = slotSize;

        TMP_Text placeholder = placeholderObject.AddComponent<TextMeshProUGUI>();
        TMP_FontAsset font = LoadChineseFont();
        if (font != null)
        {
            placeholder.font = font;
        }

        placeholder.text = GetPlaceholderText(content);
        placeholder.fontSize = 42f;
        placeholder.alignment = TextAlignmentOptions.Center;
        placeholder.color = placeholderColor;
        placeholderObject.SetActive(sprite == null);
    }

    private void ShowDetails(ShopContentDefinition content, Vector2 screenPosition)
    {
        if (content == null)
        {
            return;
        }

        RectTransform popupParent = ResolvePopupParent();
        if (popupParent == null)
        {
            return;
        }

        if (detailPopup == null)
        {
            detailPopup = ShopContentDetailPopup.GetOrCreate(popupParent, LoadChineseFont(), gameObject.layer);
        }

        detailPopup.Show(content, screenPosition);
    }

    private RectTransform ResolvePopupParent()
    {
        CanvasGroup shopGroup = GetComponentInParent<CanvasGroup>();
        if (shopGroup != null && shopGroup.transform is RectTransform groupRect)
        {
            return groupRect;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null ? canvas.transform as RectTransform : null;
    }

    private TMP_FontAsset LoadChineseFont()
    {
        if (cachedFont == null)
        {
            cachedFont = Resources.Load<TMP_FontAsset>(ChineseFontResourcePath);
        }

        return cachedFont;
    }

    private static string GetPlaceholderText(ShopContentDefinition content)
    {
        if (!string.IsNullOrWhiteSpace(content.LocalizedDisplayName))
        {
            return content.LocalizedDisplayName.Substring(0, 1);
        }

        return content.Kind == ShopContentKind.Weapon ? "武" : "道";
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
}

public sealed class BagSlotRightClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Action<Vector2> rightClickAction;

    public void Configure(Action<Vector2> action)
    {
        rightClickAction = action;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            rightClickAction?.Invoke(eventData.position);
        }
    }
}

public sealed class ShopContentDetailPopup : MonoBehaviour
{
    private static readonly Vector2 PopupSize = new Vector2(440f, 340f);

    private RectTransform popupRect;
    private Image icon;
    private TMP_Text titleText;
    private TMP_Text metaText;
    private TMP_Text detailsText;

    public static ShopContentDetailPopup GetOrCreate(
        RectTransform parent,
        TMP_FontAsset font,
        int layer)
    {
        ShopContentDetailPopup existing = parent.GetComponentInChildren<ShopContentDetailPopup>(true);
        if (existing != null)
        {
            return existing;
        }

        GameObject popupObject = new GameObject(
            "PurchasedContentDetailPopup",
            typeof(RectTransform),
            typeof(Image),
            typeof(Outline),
            typeof(ShopContentDetailPopup));
        popupObject.layer = layer;
        popupObject.transform.SetParent(parent, false);

        ShopContentDetailPopup popup = popupObject.GetComponent<ShopContentDetailPopup>();
        popup.Build(font, layer);
        popupObject.SetActive(false);
        return popup;
    }

    private void Build(TMP_FontAsset font, int layer)
    {
        popupRect = GetComponent<RectTransform>();
        popupRect.anchorMin = new Vector2(0.5f, 0.5f);
        popupRect.anchorMax = new Vector2(0.5f, 0.5f);
        popupRect.pivot = new Vector2(0.5f, 0.5f);
        popupRect.sizeDelta = PopupSize;

        Image panel = GetComponent<Image>();
        panel.color = new Color(0.035f, 0.04f, 0.05f, 0.98f);
        panel.raycastTarget = true;

        Outline outline = GetComponent<Outline>();
        outline.effectColor = new Color(0.72f, 0.76f, 0.82f, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);

        GameObject iconObject = CreateChild("Icon", layer, typeof(Image));
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 1f);
        iconRect.anchorMax = new Vector2(0f, 1f);
        iconRect.pivot = new Vector2(0f, 1f);
        iconRect.anchoredPosition = new Vector2(14f, -14f);
        iconRect.sizeDelta = new Vector2(68f, 68f);
        icon = iconObject.GetComponent<Image>();
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        titleText = CreateText("Title", font, layer, 25f, FontStyles.Bold);
        SetRect(titleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(94f, -48f), new Vector2(-48f, -10f));
        titleText.alignment = TextAlignmentOptions.Left;

        metaText = CreateText("Meta", font, layer, 17f, FontStyles.Normal);
        SetRect(metaText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(94f, -76f), new Vector2(-48f, -48f));
        metaText.alignment = TextAlignmentOptions.Left;
        metaText.color = new Color(0.75f, 0.78f, 0.84f, 1f);

        detailsText = CreateText("Details", font, layer, 18f, FontStyles.Normal);
        SetRect(detailsText.rectTransform, Vector2.zero, Vector2.one,
            new Vector2(16f, 16f), new Vector2(-16f, -94f));
        detailsText.alignment = TextAlignmentOptions.TopLeft;
        detailsText.enableWordWrapping = true;
        detailsText.enableAutoSizing = true;
        detailsText.fontSizeMin = 12f;
        detailsText.fontSizeMax = 18f;
        detailsText.overflowMode = TextOverflowModes.Ellipsis;

        GameObject closeObject = CreateChild("CloseButton", layer, typeof(Image), typeof(Button));
        RectTransform closeRect = closeObject.GetComponent<RectTransform>();
        closeRect.anchorMin = Vector2.one;
        closeRect.anchorMax = Vector2.one;
        closeRect.pivot = Vector2.one;
        closeRect.anchoredPosition = new Vector2(-8f, -8f);
        closeRect.sizeDelta = new Vector2(34f, 34f);

        Image closeImage = closeObject.GetComponent<Image>();
        closeImage.color = new Color(0.3f, 0.12f, 0.12f, 0.95f);
        Button closeButton = closeObject.GetComponent<Button>();
        closeButton.targetGraphic = closeImage;
        closeButton.onClick.AddListener(Hide);

        TMP_Text closeText = CreateText("CloseText", font, layer, 24f, FontStyles.Bold, closeObject.transform);
        SetRect(closeText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        closeText.text = "×";
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.raycastTarget = false;
    }

    public void Show(ShopContentDefinition content, Vector2 screenPosition)
    {
        if (content == null)
        {
            return;
        }

        Sprite loadedIcon = content.LoadIcon();
        icon.sprite = loadedIcon;
        icon.color = loadedIcon != null ? Color.white : Color.clear;
        titleText.text = content.LocalizedDisplayName;
        titleText.color = GetRarityColor(content.Rarity);
        metaText.text = BuildMetaLine(content);

        string details = content.BuildDetails();
        detailsText.text = string.IsNullOrWhiteSpace(details) ? "暂无详细说明" : details;

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        PositionAt(screenPosition);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    private void PositionAt(Vector2 screenPosition)
    {
        RectTransform parentRect = transform.parent as RectTransform;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (parentRect == null || canvas == null)
        {
            return;
        }

        Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                screenPosition,
                eventCamera,
                out Vector2 localPoint))
        {
            return;
        }

        Vector2 desired = localPoint + new Vector2(PopupSize.x * 0.5f + 12f, -PopupSize.y * 0.5f - 12f);
        Rect bounds = parentRect.rect;
        desired.x = Mathf.Clamp(desired.x, bounds.xMin + PopupSize.x * 0.5f, bounds.xMax - PopupSize.x * 0.5f);
        desired.y = Mathf.Clamp(desired.y, bounds.yMin + PopupSize.y * 0.5f, bounds.yMax - PopupSize.y * 0.5f);
        popupRect.anchoredPosition = desired;
    }

    private GameObject CreateChild(string objectName, int layer, params Type[] componentTypes)
    {
        Type[] types = new Type[componentTypes.Length + 1];
        types[0] = typeof(RectTransform);
        Array.Copy(componentTypes, 0, types, 1, componentTypes.Length);
        GameObject child = new GameObject(objectName, types);
        child.layer = layer;
        child.transform.SetParent(transform, false);
        return child;
    }

    private TMP_Text CreateText(
        string objectName,
        TMP_FontAsset font,
        int layer,
        float fontSize,
        FontStyles style,
        Transform parent = null)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = layer;
        textObject.transform.SetParent(parent != null ? parent : transform, false);
        TMP_Text text = textObject.GetComponent<TextMeshProUGUI>();
        if (font != null)
        {
            text.font = font;
        }

        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static void SetRect(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private static string BuildMetaLine(ShopContentDefinition content)
    {
        string kind = ShopLocalization.GetKindLabel(content.Kind);
        ShopWeaponDefinition weapon = content as ShopWeaponDefinition;
        if (weapon != null && !string.IsNullOrWhiteSpace(weapon.LocalizedClassTags))
        {
            kind += $" · {weapon.LocalizedClassTags}";
        }

        return $"{kind} · {content.RarityLabel}";
    }

    private static Color GetRarityColor(ShopRarity rarity)
    {
        switch (rarity)
        {
            case ShopRarity.Tier2:
                return new Color(0.35f, 0.95f, 0.5f, 1f);
            case ShopRarity.Tier3:
                return new Color(0.4f, 0.65f, 1f, 1f);
            case ShopRarity.Tier4:
                return new Color(0.85f, 0.45f, 1f, 1f);
            default:
                return Color.white;
        }
    }
}
