using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerExperienceHudView : MonoBehaviour
{
    private const string FontResourcePath = "Fonts & Materials/SmileySans-Oblique SDF";

    private PlayerExperience experience;
    private TMP_Text levelText;
    private TMP_Text experienceText;
    private Image fillImage;

    private void OnDestroy()
    {
        Bind(null);
    }

    public static PlayerExperienceHudView GetOrCreate(PlayerExperience playerExperience)
    {
        Transform playerState = FindPlayerState();
        PlayerExperienceHudView existing = playerState != null
            ? playerState.GetComponent<PlayerExperienceHudView>()
            : FindObjectOfType<PlayerExperienceHudView>(true);

        if (existing == null)
        {
            if (playerState == null)
            {
                Debug.LogWarning("PlayerExperienceHudView could not find PlayerState, so the experience bar was not created.");
                return null;
            }

            existing = playerState.gameObject.AddComponent<PlayerExperienceHudView>();
        }

        existing.EnsureUi();
        existing.Bind(playerExperience);
        return existing;
    }

    public void Bind(PlayerExperience playerExperience)
    {
        if (experience == playerExperience)
        {
            Refresh();
            return;
        }

        if (experience != null)
        {
            experience.ExperienceChanged -= HandleExperienceChanged;
        }

        experience = playerExperience;
        if (experience != null)
        {
            experience.ExperienceChanged += HandleExperienceChanged;
        }

        Refresh();
    }

    private void HandleExperienceChanged(PlayerExperience changedExperience)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (experience == null || levelText == null || experienceText == null || fillImage == null)
        {
            return;
        }

        levelText.text = $"Lv.{experience.Level}";
        experienceText.text = $"{experience.CurrentExperienceDisplay} / {experience.ExperienceRequired}";
        fillImage.fillAmount = experience.Progress;
    }

    private void EnsureUi()
    {
        BindUiReferences();
        if (levelText == null || experienceText == null || fillImage == null)
        {
            BuildUi();
        }
    }

    private void BindUiReferences()
    {
        Transform root = transform.Find("Experience");
        if (root == null)
        {
            return;
        }

        levelText = root.Find("Level")?.GetComponent<TMP_Text>();
        experienceText = root.Find("Value")?.GetComponent<TMP_Text>()
            ?? root.Find("Bar/Value")?.GetComponent<TMP_Text>();
        fillImage = root.Find("Fill")?.GetComponent<Image>()
            ?? root.Find("Bar/Fill")?.GetComponent<Image>();
    }

    private void BuildUi()
    {
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>(FontResourcePath);
        Transform oldRoot = transform.Find("Experience");
        if (oldRoot != null)
        {
            Destroy(oldRoot.gameObject);
        }

        GameObject root = CreateUiObject("Experience", transform);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        RectTransform healthRect = transform.Find("Blood") as RectTransform;
        if (healthRect != null)
        {
            CopyRectLayout(healthRect, rootRect);
            rootRect.anchoredPosition += Vector2.down * (healthRect.sizeDelta.y + 125f);
            rootRect.sizeDelta = new Vector2(healthRect.sizeDelta.x, 64f);
        }
        else
        {
            rootRect.anchorMin = new Vector2(0f, 1f);
            rootRect.anchorMax = new Vector2(0f, 1f);
            rootRect.pivot = new Vector2(0f, 1f);
            rootRect.anchoredPosition = new Vector2(24f, -220f);
            rootRect.sizeDelta = new Vector2(600f, 64f);
        }

        Image rootImage = root.AddComponent<Image>();
        rootImage.color = new Color(0.04f, 0.045f, 0.06f, 0.9f);
        rootImage.raycastTarget = false;

        levelText = CreateText("Level", root.transform, font, 24f, FontStyles.Bold);
        SetRect(levelText.rectTransform, new Vector2(0.02f, 0.12f), new Vector2(0.24f, 0.88f));
        levelText.alignment = TextAlignmentOptions.Center;

        GameObject bar = CreateUiObject("Bar", root.transform);
        SetRect(bar.GetComponent<RectTransform>(), new Vector2(0.27f, 0.22f), new Vector2(0.96f, 0.78f));
        Image barImage = bar.AddComponent<Image>();
        barImage.color = new Color(0.12f, 0.13f, 0.16f, 1f);

        GameObject fill = CreateUiObject("Fill", bar.transform);
        Stretch(fill.GetComponent<RectTransform>());
        fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.22f, 0.88f, 0.32f, 1f);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0;
        Image healthFill = transform.Find("Blood/Fill")?.GetComponent<Image>();
        if (healthFill != null)
        {
            fillImage.sprite = healthFill.sprite;
        }

        experienceText = CreateText("Value", bar.transform, font, 18f, FontStyles.Bold);
        Stretch(experienceText.rectTransform);
        experienceText.alignment = TextAlignmentOptions.Center;
    }

    private static Transform FindPlayerState()
    {
        PlayerHealthBarView healthView = FindObjectOfType<PlayerHealthBarView>(true);
        if (healthView != null)
        {
            return healthView.transform;
        }

        GameObject playerState = GameObject.Find("PlayerState");
        return playerState != null ? playerState.transform : null;
    }

    private static void CopyRectLayout(RectTransform source, RectTransform destination)
    {
        destination.anchorMin = source.anchorMin;
        destination.anchorMax = source.anchorMax;
        destination.pivot = source.pivot;
        destination.anchoredPosition = source.anchoredPosition;
        destination.sizeDelta = source.sizeDelta;
        destination.localRotation = source.localRotation;
        destination.localScale = source.localScale;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject result = new GameObject(objectName, typeof(RectTransform));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static TMP_Text CreateText(
        string objectName,
        Transform parent,
        TMP_FontAsset font,
        float fontSize,
        FontStyles style)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
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

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Stretch(RectTransform rect)
    {
        SetRect(rect, Vector2.zero, Vector2.one);
    }
}
