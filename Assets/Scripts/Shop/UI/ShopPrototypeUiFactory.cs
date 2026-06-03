using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ShopPrototypeUiFactory
{
    private static TMP_FontAsset chineseFont;
    private static bool chineseFontLoaded;

    public static void Build(ShopManager manager)
    {
        EnsureEventSystem();

        GameObject canvasObject = new GameObject(
            "Prototype Shop Canvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(manager.transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform panel = CreatePanel(
            "Shop Panel",
            canvasObject.transform,
            new Vector2(0f, 0f),
            new Vector2(1220f, 620f),
            new Color(0.075f, 0.075f, 0.09f, 0.97f));

        CreateText(
            "Title",
            panel,
            "土豆商店原型",
            new Vector2(0f, 270f),
            new Vector2(600f, 46f),
            30,
            TextAlignmentOptions.Center,
            Color.white);

        CreateText(
            "Subtitle",
            panel,
            "当前阶段：刷新商品。购买和背包功能稍后接入。",
            new Vector2(0f, 236f),
            new Vector2(760f, 30f),
            17,
            TextAlignmentOptions.Center,
            new Color(0.78f, 0.78f, 0.82f, 1f));

        var views = new List<ShopOfferView>();
        const int cardCount = 5;
        const float cardWidth = 220f;
        const float cardSpacing = 16f;
        float firstCardX = -((cardCount - 1) * (cardWidth + cardSpacing)) * 0.5f;

        for (int index = 0; index < cardCount; index++)
        {
            views.Add(CreateOfferCard(
                panel,
                index + 1,
                new Vector2(firstCardX + index * (cardWidth + cardSpacing), 32f),
                new Vector2(cardWidth, 360f)));
        }

        TMP_Text statusText = CreateText(
            "Status",
            panel,
            string.Empty,
            new Vector2(-200f, -235f),
            new Vector2(760f, 72f),
            16,
            TextAlignmentOptions.Left,
            new Color(0.86f, 0.86f, 0.90f, 1f));

        Button refreshButton = CreateButton(
            "Refresh Button",
            panel,
            "刷新",
            new Vector2(455f, -235f),
            new Vector2(230f, 64f),
            new Color(0.66f, 0.39f, 0.15f, 1f));

        manager.ConfigureUi(views.ToArray(), refreshButton, statusText);
    }

    private static ShopOfferView CreateOfferCard(
        Transform parent,
        int cardNumber,
        Vector2 anchoredPosition,
        Vector2 size)
    {
        RectTransform card = CreatePanel(
            $"Offer Card {cardNumber}",
            parent,
            anchoredPosition,
            size,
            new Color(0.27f, 0.27f, 0.27f, 0.98f));

        Image background = card.GetComponent<Image>();
        Button inspectButton = card.gameObject.AddComponent<Button>();
        inspectButton.targetGraphic = background;

        RectTransform iconRect = CreatePanel(
            "Icon",
            card,
            new Vector2(0f, 108f),
            new Vector2(82f, 82f),
            new Color(0.15f, 0.30f, 0.22f, 1f));
        Image icon = iconRect.GetComponent<Image>();
        TMP_Text iconPlaceholder = CreateText(
            "Icon Placeholder",
            iconRect,
            "?",
            Vector2.zero,
            new Vector2(70f, 70f),
            42,
            TextAlignmentOptions.Center,
            new Color(0.95f, 0.95f, 0.95f, 1f));

        TMP_Text nameText = CreateText(
            "Name",
            card,
            "Offer",
            new Vector2(0f, 50f),
            new Vector2(196f, 42f),
            21,
            TextAlignmentOptions.Center,
            Color.white);

        TMP_Text kindText = CreateText(
            "Kind",
            card,
            string.Empty,
            new Vector2(0f, 19f),
            new Vector2(196f, 24f),
            13,
            TextAlignmentOptions.Center,
            new Color(0.88f, 0.77f, 0.48f, 1f));

        TMP_Text descriptionText = CreateText(
            "Description",
            card,
            string.Empty,
            new Vector2(0f, -44f),
            new Vector2(190f, 92f),
            14,
            TextAlignmentOptions.TopLeft,
            new Color(0.93f, 0.93f, 0.93f, 1f));

        TMP_Text statsText = CreateText(
            "Stats",
            card,
            string.Empty,
            new Vector2(0f, -112f),
            new Vector2(190f, 46f),
            12,
            TextAlignmentOptions.Left,
            new Color(0.74f, 0.84f, 0.75f, 1f));

        TMP_Text priceText = CreateText(
            "Price",
            card,
            string.Empty,
            new Vector2(0f, -158f),
            new Vector2(190f, 28f),
            16,
            TextAlignmentOptions.Center,
            new Color(1f, 0.83f, 0.34f, 1f));

        ShopOfferView view = card.gameObject.AddComponent<ShopOfferView>();
        view.Configure(
            background,
            icon,
            iconPlaceholder,
            nameText,
            kindText,
            descriptionText,
            statsText,
            priceText,
            inspectButton);
        return view;
    }

    private static RectTransform CreatePanel(
        string name,
        Transform parent,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color)
    {
        GameObject panelObject = CreateUiObject(name, parent);
        RectTransform rect = panelObject.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image image = panelObject.AddComponent<Image>();
        image.color = color;
        return rect;
    }

    private static Button CreateButton(
        string name,
        Transform parent,
        string label,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color)
    {
        RectTransform buttonRect = CreatePanel(name, parent, anchoredPosition, size, color);
        Image image = buttonRect.GetComponent<Image>();
        Button button = buttonRect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;

        CreateText(
            "Label",
            buttonRect,
            label,
            Vector2.zero,
            size,
            23,
            TextAlignmentOptions.Center,
            Color.white);
        return button;
    }

    private static TMP_Text CreateText(
        string name,
        Transform parent,
        string value,
        Vector2 anchoredPosition,
        Vector2 size,
        int fontSize,
        TextAlignmentOptions alignment,
        Color color)
    {
        GameObject textObject = CreateUiObject(name, parent);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        TMP_FontAsset loadedFont = LoadChineseFont();
        if (loadedFont != null)
        {
            text.font = loadedFont;
        }

        text.text = value;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Truncate;
        return text;
    }

    private static TMP_FontAsset LoadChineseFont()
    {
        if (!chineseFontLoaded)
        {
            chineseFontLoaded = true;
            chineseFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/SmileySans-Oblique SDF");
        }

        return chineseFont;
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        var uiObject = new GameObject(name, typeof(RectTransform));
        uiObject.layer = 5;
        uiObject.transform.SetParent(parent, false);

        RectTransform rect = uiObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        return uiObject;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        var eventSystem = new GameObject(
            "EventSystem",
            typeof(EventSystem),
            typeof(StandaloneInputModule));
        Object.DontDestroyOnLoad(eventSystem);
    }
}
