using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        contents.Add(content);
        CreateSlot(content);
    }

    protected void RebuildSlotViews()
    {
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
