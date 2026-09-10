using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class ShopOfferView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static readonly Color AffordablePaperColor = new Color(0.88f, 0.79f, 0.61f, 1f);
    private static readonly Color UnaffordablePaperColor = new Color(0.62f, 0.55f, 0.45f, 0.9f);
    private static readonly Color PaperTextColor = new Color(0.22f, 0.16f, 0.12f, 1f);

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
    [SerializeField] private Button lockButton;
    [SerializeField] private TMP_Text lockText;

    [Header("Paper Motion")]
    [SerializeField, Min(0f)] private float dropDistance = 260f;
    [SerializeField, Min(0.01f)] private float dropDuration = 0.42f;
    [SerializeField, Min(0.01f)] private float purchaseDropDuration = 0.28f;
    [SerializeField, Min(0.01f)] private float pullReturnDuration = 0.18f;
    [SerializeField] private float restingTilt = 1.25f;
    [SerializeField, Min(1f)] private float pullPurchaseDistance = 135f;
    [SerializeField, Min(1f)] private float maximumPullDistance = 210f;

    [Header("Pin Motion")]
    [SerializeField, Min(0f)] private float pinInsertDistance = 16f;
    [SerializeField, Min(0.01f)] private float pinMotionDuration = 0.18f;
    [SerializeField] private float loosePinAngle = -10f;

    private ShopContentDefinition content;
    private Action<ShopContentDefinition> inspectAction;
    private Action<ShopOfferView, ShopContentDefinition> buyAction;
    private Action<ShopOfferView, bool> lockAction;
    private RectTransform cardRect;
    private RectTransform pinRect;
    private Canvas rootCanvas;
    private Vector2 restingPosition;
    private Quaternion restingRotation;
    private Vector3 restingScale;
    private Vector2 loosePinPosition;
    private Coroutine paperAnimation;
    private Coroutine pinAnimation;
    private bool poseCached;
    private bool pinPoseCached;
    private bool dragging;
    private bool purchaseAnimating;
    private bool clearCardWhenDisabled;
    private float pulledDistance;

    public bool IsLocked { get; private set; }
    public bool HasSceneReferences => background != null && icon != null && nameText != null
        && kindText != null && descriptionText != null && statsText != null && priceText != null
        && inspectButton != null && buyButton != null && lockButton != null && lockText != null;

    private void Awake()
    {
        AutoBindReferences();
        CachePoses();
        BindInspectButton();
        BindBuyButton();
        BindLockButton();
    }

    private void Reset() => AutoBindReferences();

    private void OnValidate()
    {
        AutoBindReferences();
        maximumPullDistance = Mathf.Max(maximumPullDistance, pullPurchaseDistance);
    }

    private void OnDisable()
    {
        StopPaperAnimation();
        StopPinAnimation();
        dragging = false;
        purchaseAnimating = false;
        if (clearCardWhenDisabled)
        {
            clearCardWhenDisabled = false;
            MarkPurchased();
        }
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        background ??= GetComponent<Image>();
        IconPanel ??= FindDescendant("IconPanel")?.gameObject;
        background ??= FindComponent<Image>("Background");
        icon ??= FindComponent<Image>("Icon");
        iconPlaceholder ??= FindComponent<TMP_Text>("IconPlaceholder", "Icon Placeholder");
        nameText ??= FindComponent<TMP_Text>("NameText", "Name");
        kindText ??= FindComponent<TMP_Text>("KindText", "Kind");
        limitText ??= FindComponent<TMP_Text>("LimitText", "Limit");
        descriptionText ??= FindComponent<TMP_Text>("DescriptionText", "Description");
        statsText ??= FindComponent<TMP_Text>("StatsText", "Stats");
        priceText ??= FindComponent<TMP_Text>("PriceText", "Price");
        inspectButton ??= GetComponent<Button>();
        inspectButton ??= FindComponent<Button>("InspectButton");
        buyButton ??= FindComponent<Button>("PricePanel", "BuyButton", "PriceButton");
        lockButton ??= FindComponent<Button>("LockButton", "Lock Button");
        if (lockText == null && lockButton != null)
        {
            lockText = lockButton.GetComponentInChildren<TMP_Text>(true);
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
        AutoBindReferences();
        CachePoses();
        BindInspectButton();
        BindBuyButton();
        BindLockButton();
    }

    private void BindInspectButton()
    {
        if (inspectButton == null) return;
        inspectButton.targetGraphic ??= background;
        inspectButton.onClick.RemoveListener(Inspect);
        inspectButton.onClick.AddListener(Inspect);
    }

    private void BindBuyButton()
    {
        if (buyButton == null) return;
        buyButton.onClick.RemoveListener(Buy);
        buyButton.interactable = false;
        buyButton.transition = Selectable.Transition.None;
    }

    private void BindLockButton()
    {
        if (lockButton == null) return;
        lockButton.onClick.RemoveListener(ToggleLock);
        lockButton.onClick.AddListener(ToggleLock);
    }

    public void Bind(
        ShopContentDefinition newContent,
        Action<ShopContentDefinition> newInspectAction,
        Action<ShopOfferView, ShopContentDefinition> newBuyAction = null,
        int purchaseCount = 0,
        Action<ShopOfferView, bool> newLockAction = null,
        bool initiallyLocked = false,
        int displayedPrice = -1,
        bool canAfford = true)
    {
        StopPaperAnimation();
        CachePoses();
        RestoreRestingPose();
        content = newContent;
        inspectAction = newInspectAction;
        buyAction = newBuyAction;
        lockAction = newLockAction;
        purchaseAnimating = false;
        clearCardWhenDisabled = false;
        SetLocked(initiallyLocked, false);

        if (content == null)
        {
            MarkPurchased();
            return;
        }

        SetCardContentVisible(true);
        if (inspectButton != null) inspectButton.interactable = true;
        if (lockButton != null) lockButton.interactable = true;

        Sprite loadedIcon = content.LoadIcon();
        if (icon != null)
        {
            icon.sprite = loadedIcon;
            icon.color = loadedIcon != null ? Color.white : GetPlaceholderColor(content.Kind);
        }
        if (background != null) background.color = GetRarityColor(content.Rarity);
        if (iconPlaceholder != null)
        {
            iconPlaceholder.gameObject.SetActive(loadedIcon == null);
            iconPlaceholder.text = content.Kind == ShopContentKind.Weapon ? "武" : "道";
        }
        if (nameText != null) nameText.text = content.LocalizedDisplayName;
        if (kindText != null) kindText.text = BuildKindLabel(content);
        if (descriptionText != null)
        {
            descriptionText.text = string.IsNullOrWhiteSpace(content.LocalizedDescription)
                ? string.Empty
                : $"“{content.LocalizedDescription}”";
            descriptionText.gameObject.SetActive(!string.IsNullOrWhiteSpace(descriptionText.text));
        }
        if (statsText != null) statsText.text = content.BuildStatLine();

        SetPurchaseState(displayedPrice >= 0 ? displayedPrice : content.BasePrice, canAfford);
        BindLimit(content, purchaseCount);
        ApplyPaperTextColors();
    }

    public void SetPrice(int price) => SetPurchaseState(price, true);

    public void SetPurchaseState(int price, bool canAfford)
    {
        if (priceText != null)
        {
            string priceColor = canAfford ? "#4F7A3A" : "#A33D32";
            priceText.text = $"下拉购买 ↓\n<color={priceColor}>{Mathf.Max(0, price)}</color> 材料";
            priceText.color = PaperTextColor;
        }
        if (buyButton?.targetGraphic != null)
        {
            buyButton.targetGraphic.color = canAfford ? AffordablePaperColor : UnaffordablePaperColor;
        }
    }

    public void SetVisible(bool visible) => gameObject.SetActive(visible);

    public void PlayDropIn(float delay = 0f)
    {
        if (!isActiveAndEnabled || content == null) return;
        StopPaperAnimation();
        CachePoses();
        paperAnimation = StartCoroutine(AnimateDropIn(Mathf.Max(0f, delay)));
    }

    public void PlayPurchasedDrop()
    {
        if (content == null || purchaseAnimating) return;
        StopPaperAnimation();
        dragging = false;
        purchaseAnimating = true;
        clearCardWhenDisabled = true;
        if (inspectButton != null) inspectButton.interactable = false;
        if (lockButton != null) lockButton.interactable = false;
        paperAnimation = StartCoroutine(AnimatePurchasedDrop());
    }

    public void MarkPurchased()
    {
        StopPaperAnimation();
        content = null;
        inspectAction = null;
        buyAction = null;
        lockAction = null;
        purchaseAnimating = false;
        clearCardWhenDisabled = false;
        SetLocked(false, false);
        RestoreRestingPose();
        SetCardContentVisible(false);
        if (background != null) background.color = Color.clear;
        if (inspectButton != null) inspectButton.interactable = false;
        if (lockButton != null) lockButton.interactable = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (content == null || purchaseAnimating || IsPointerOnPin(eventData)) return;
        StopPaperAnimation();
        CachePoses();
        dragging = true;
        pulledDistance = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || cardRect == null) return;
        float scaleFactor = rootCanvas != null ? Mathf.Max(0.01f, rootCanvas.scaleFactor) : 1f;
        pulledDistance = Mathf.Clamp(pulledDistance - eventData.delta.y / scaleFactor, 0f, maximumPullDistance);
        float progress = pulledDistance / Mathf.Max(1f, pullPurchaseDistance);
        cardRect.anchoredPosition = restingPosition + Vector2.down * pulledDistance;
        cardRect.localRotation = restingRotation * Quaternion.Euler(0f, 0f, Mathf.Sin(progress * Mathf.PI) * 2.5f);
        cardRect.localScale = restingScale * Mathf.Lerp(1f, 0.97f, Mathf.Clamp01(progress));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging) return;
        dragging = false;
        if (pulledDistance >= pullPurchaseDistance && buyAction != null)
        {
            buyAction.Invoke(this, content);
            if (purchaseAnimating) return;
        }
        paperAnimation = StartCoroutine(AnimateBackToRest());
    }

    private void Inspect()
    {
        if (!dragging && !purchaseAnimating) inspectAction?.Invoke(content);
    }

    private void Buy() { }

    private void ToggleLock()
    {
        if (content != null && !purchaseAnimating) SetLocked(!IsLocked, true);
    }

    public void SetLocked(bool locked, bool notify)
    {
        bool nextValue = locked && content != null;
        bool changed = nextValue != IsLocked;
        IsLocked = nextValue;
        UpdateLockVisual(notify && changed && isActiveAndEnabled);
        if (notify) lockAction?.Invoke(this, IsLocked);
    }

    private void UpdateLockVisual(bool animate)
    {
        CachePoses();
        if (lockText != null) lockText.gameObject.SetActive(false);
        if (lockButton?.targetGraphic != null) lockButton.targetGraphic.color = Color.white;
        if (pinRect == null) return;
        StopPinAnimation();
        if (animate) pinAnimation = StartCoroutine(AnimatePin(IsLocked));
        else ApplyPinPose(IsLocked);
    }

    private IEnumerator AnimateDropIn(float delay)
    {
        Vector2 start = restingPosition + Vector2.up * dropDistance;
        cardRect.anchoredPosition = start;
        cardRect.localRotation = restingRotation * Quaternion.Euler(0f, 0f, -5f);
        cardRect.localScale = restingScale * 0.98f;
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);

        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / dropDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            float bounce = Mathf.Sin(t * Mathf.PI * 2f) * (1f - t) * 8f;
            cardRect.anchoredPosition = Vector2.Lerp(start, restingPosition, eased) + Vector2.up * bounce;
            cardRect.localRotation = Quaternion.Slerp(restingRotation * Quaternion.Euler(0f, 0f, -5f), restingRotation, eased);
            cardRect.localScale = Vector3.Lerp(restingScale * 0.98f, restingScale, eased);
            yield return null;
        }
        RestoreRestingPose();
        paperAnimation = null;
    }

    private IEnumerator AnimateBackToRest()
    {
        Vector2 startPosition = cardRect.anchoredPosition;
        Quaternion startRotation = cardRect.localRotation;
        Vector3 startScale = cardRect.localScale;
        float elapsed = 0f;
        while (elapsed < pullReturnDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = 1f - Mathf.Pow(1f - Mathf.Clamp01(elapsed / pullReturnDuration), 3f);
            cardRect.anchoredPosition = Vector2.Lerp(startPosition, restingPosition, t);
            cardRect.localRotation = Quaternion.Slerp(startRotation, restingRotation, t);
            cardRect.localScale = Vector3.Lerp(startScale, restingScale, t);
            yield return null;
        }
        RestoreRestingPose();
        paperAnimation = null;
    }

    private IEnumerator AnimatePurchasedDrop()
    {
        Vector2 startPosition = cardRect.anchoredPosition;
        Quaternion startRotation = cardRect.localRotation;
        Vector3 startScale = cardRect.localScale;
        Vector2 endPosition = restingPosition + Vector2.down * (dropDistance + maximumPullDistance);
        float elapsed = 0f;
        while (elapsed < purchaseDropDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / purchaseDropDuration);
            float eased = t * t;
            cardRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, eased);
            cardRect.localRotation = Quaternion.Slerp(startRotation, restingRotation * Quaternion.Euler(0f, 0f, 8f), eased);
            cardRect.localScale = Vector3.Lerp(startScale, restingScale * 0.94f, eased);
            yield return null;
        }
        paperAnimation = null;
        clearCardWhenDisabled = false;
        MarkPurchased();
    }

    private IEnumerator AnimatePin(bool insert)
    {
        Vector2 startPosition = pinRect.anchoredPosition;
        Quaternion startRotation = pinRect.localRotation;
        Vector3 startScale = pinRect.localScale;
        Vector2 targetPosition = loosePinPosition + (insert ? Vector2.down * pinInsertDistance : Vector2.zero);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, insert ? 0f : loosePinAngle);
        Vector3 targetScale = Vector3.one * (insert ? 0.88f : 1f);
        float elapsed = 0f;
        while (elapsed < pinMotionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / pinMotionDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            float bounce = Mathf.Sin(t * Mathf.PI) * (insert ? 4f : -4f);
            pinRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, eased) + Vector2.up * bounce;
            pinRect.localRotation = Quaternion.Slerp(startRotation, targetRotation, eased);
            pinRect.localScale = Vector3.Lerp(startScale, targetScale, eased);
            yield return null;
        }
        ApplyPinPose(insert);
        pinAnimation = null;
    }

    private void CachePoses()
    {
        if (!poseCached)
        {
            cardRect = transform as RectTransform;
            rootCanvas = GetComponentInParent<Canvas>();
            if (cardRect != null)
            {
                restingPosition = cardRect.anchoredPosition;
                float direction = transform.GetSiblingIndex() % 2 == 0 ? -1f : 1f;
                restingRotation = Quaternion.Euler(0f, 0f, restingTilt * direction);
                restingScale = cardRect.localScale;
                poseCached = true;
            }
        }
        if (!pinPoseCached && lockButton != null)
        {
            pinRect = lockButton.transform as RectTransform;
            if (pinRect != null)
            {
                loosePinPosition = pinRect.anchoredPosition;
                pinPoseCached = true;
            }
        }
    }

    private void RestoreRestingPose()
    {
        if (cardRect == null) return;
        cardRect.anchoredPosition = restingPosition;
        cardRect.localRotation = restingRotation;
        cardRect.localScale = restingScale;
        pulledDistance = 0f;
    }

    private void ApplyPinPose(bool inserted)
    {
        if (pinRect == null) return;
        pinRect.anchoredPosition = loosePinPosition + (inserted ? Vector2.down * pinInsertDistance : Vector2.zero);
        pinRect.localRotation = Quaternion.Euler(0f, 0f, inserted ? 0f : loosePinAngle);
        pinRect.localScale = Vector3.one * (inserted ? 0.88f : 1f);
    }

    private void StopPaperAnimation()
    {
        if (paperAnimation == null) return;
        StopCoroutine(paperAnimation);
        paperAnimation = null;
    }

    private void StopPinAnimation()
    {
        if (pinAnimation == null) return;
        StopCoroutine(pinAnimation);
        pinAnimation = null;
    }

    private bool IsPointerOnPin(PointerEventData eventData)
    {
        if (lockButton == null || eventData?.pointerPress == null) return false;
        Transform pressed = eventData.pointerPress.transform;
        return pressed == lockButton.transform || pressed.IsChildOf(lockButton.transform);
    }

    private void ApplyPaperTextColors()
    {
        SetTextColor(nameText, PaperTextColor);
        SetTextColor(kindText, PaperTextColor);
        SetTextColor(limitText, PaperTextColor);
        SetTextColor(descriptionText, PaperTextColor);
        SetTextColor(statsText, PaperTextColor);
    }

    private static void SetTextColor(TMP_Text text, Color color)
    {
        if (text != null) text.color = color;
    }

    private void SetCardContentVisible(bool visible)
    {
        if (IconPanel != null) IconPanel.SetActive(visible);
        SetGameObjectActive(icon, visible);
        SetGameObjectActive(iconPlaceholder, visible);
        SetGameObjectActive(nameText, visible);
        SetGameObjectActive(kindText, visible);
        SetGameObjectActive(limitText, visible);
        SetGameObjectActive(descriptionText, visible);
        SetGameObjectActive(statsText, visible);
        SetGameObjectActive(priceText, visible);
        if (buyButton != null) buyButton.gameObject.SetActive(visible);
        if (lockButton != null) lockButton.gameObject.SetActive(visible);
    }

    private static void SetGameObjectActive(Component component, bool active)
    {
        if (component != null) component.gameObject.SetActive(active);
    }

    private void BindLimit(ShopContentDefinition newContent, int purchaseCount)
    {
        if (limitText == null) return;
        ShopItemDefinition item = newContent as ShopItemDefinition;
        bool hasLimit = item != null && item.PurchaseLimit > 0;
        limitText.gameObject.SetActive(hasLimit);
        if (hasLimit)
        {
            limitText.text = $"限制 ({Mathf.Clamp(purchaseCount, 0, item.PurchaseLimit)}/{item.PurchaseLimit})";
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
            case ShopRarity.Tier2: return new Color(0.86f, 0.91f, 0.72f, 1f);
            case ShopRarity.Tier3: return new Color(0.77f, 0.84f, 0.91f, 1f);
            case ShopRarity.Tier4: return new Color(0.89f, 0.77f, 0.89f, 1f);
            default: return new Color(0.91f, 0.84f, 0.70f, 1f);
        }
    }

    private Transform FindDescendant(params string[] names)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (child.name == objectName) return child;
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
