using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShopBagBase : MonoBehaviour
{
    [Header("Scene UI")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private ShopBagSlotView[] slotViews = Array.Empty<ShopBagSlotView>();
    [SerializeField] private ShopContentDetailPopup detailPopup;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private TMP_Text pageText;

    private readonly List<ShopContentDefinition> contents = new List<ShopContentDefinition>();
    private int currentPage;
    private int PageCount => slotViews.Length == 0 ? 1 : Mathf.Max(1, (Count + slotViews.Length - 1) / slotViews.Length);

    public IReadOnlyList<ShopContentDefinition> Contents => contents;
    public int Count => contents.Count;
    protected List<ShopContentDefinition> MutableContents => contents;
    public event Action ContentsChanged;

    protected virtual string MissingBagMessage => "背包未设置。";

    protected virtual void Awake()
    {
        AutoBindReferences();
        if (!HasSceneReferences())
        {
            Debug.LogError("Shop bag slots and detail panel must be assigned in the scene.", this);
            return;
        }
        if (previousPageButton != null) previousPageButton.onClick.AddListener(PreviousPage);
        if (nextPageButton != null) nextPageButton.onClick.AddListener(NextPage);
        RebuildSlotViews();
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

        if (!HasSceneReferences())
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

        if (!HasSceneReferences())
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

    public void RestoreContentIds(IReadOnlyList<string> contentIds)
    {
        AutoBindReferences();
        contents.Clear();
        if (contentIds != null)
        {
            for (int index = 0; index < contentIds.Count; index++)
            {
                ShopContentDefinition content = ShopContentCatalog.FindById(contentIds[index]);
                if (content != null && CanAdd(content, out _))
                {
                    contents.Add(content);
                }
            }
        }

        NormalizeRestoredContents();
        RebuildSlotViews();
        ContentsChanged?.Invoke();
    }

    protected virtual void NormalizeRestoredContents()
    {
    }

    protected virtual void StoreContent(ShopContentDefinition content)
    {
        detailPopup?.Hide();
        contents.Add(content);
        currentPage = PageCount - 1;
        RebuildSlotViews();
    }

    protected void RebuildSlotViews()
    {
        detailPopup?.Hide();
        AutoBindReferences();
        if (!HasSceneReferences())
        {
            return;
        }

        currentPage = Mathf.Clamp(currentPage, 0, PageCount - 1);
        for (int index = 0; index < slotViews.Length; index++)
        {
            int contentIndex = currentPage * slotViews.Length + index;
            ShopContentDefinition content = contentIndex < Count ? contents[contentIndex] : null;
            slotViews[index].Bind(content, content != null ? GetFallbackIcon(content) : null, detailPopup);
        }
        if (previousPageButton != null) previousPageButton.interactable = currentPage > 0;
        if (nextPageButton != null) nextPageButton.interactable = currentPage + 1 < PageCount;
        if (pageText != null) pageText.text = $"{currentPage + 1} / {PageCount}";
    }

    private void PreviousPage()
    {
        currentPage--;
        RebuildSlotViews();
    }

    private bool HasSceneReferences()
    {
        return contentRoot != null && detailPopup != null && slotViews != null && slotViews.Length > 0
            && !Array.Exists(slotViews, view => view == null || !view.HasSceneReferences);
    }

    private void NextPage()
    {
        currentPage++;
        RebuildSlotViews();
    }

    protected abstract bool CanAdd(ShopContentDefinition content, out string failureReason);

    protected virtual Sprite GetFallbackIcon(ShopContentDefinition content)
    {
        return null;
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
