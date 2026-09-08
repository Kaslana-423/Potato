using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_Dropdown))]
public sealed class ResolutionDropdownSetting : MonoBehaviour
{
    private static readonly string[] OptionLabels =
    {
        "1920 × 1080",
        "1280 × 720",
        "960 × 540",
        "全屏"
    };

    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        EnsureOptions(dropdown);
        dropdown.onValueChanged.AddListener(HandleValueChanged);
    }

    private void OnEnable()
    {
        if (dropdown == null)
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }

        dropdown.SetValueWithoutNotify((int)GameSessionState.ResolutionMode);
        dropdown.RefreshShownValue();
    }

    private void OnDestroy()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(HandleValueChanged);
        }
    }

    private static void HandleValueChanged(int selectedIndex)
    {
        GameSessionState.SetResolutionMode((GameResolutionMode)selectedIndex);
    }

    private static void EnsureOptions(TMP_Dropdown target)
    {
        bool matches = target.options.Count == OptionLabels.Length;
        for (int i = 0; matches && i < OptionLabels.Length; i++)
        {
            matches = target.options[i].text == OptionLabels[i];
        }

        if (matches)
        {
            return;
        }

        target.ClearOptions();
        target.AddOptions(new List<string>(OptionLabels));
        target.RefreshShownValue();
    }

#if UNITY_EDITOR
    public static TMP_Dropdown CreateSceneDropdown(Transform parent, TMP_FontAsset font)
    {
        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources
        {
            standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
            background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"),
            inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd"),
            knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"),
            checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd"),
            dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd"),
            mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd")
        };

        GameObject dropdownObject = TMP_DefaultControls.CreateDropdown(resources);
        dropdownObject.name = "ResolutionDropdown";
        dropdownObject.layer = parent.gameObject.layer;
        dropdownObject.transform.SetParent(parent, false);

        TMP_Dropdown result = dropdownObject.GetComponent<TMP_Dropdown>();
        EnsureOptions(result);
        ConfigureText(result.captionText, font, 24f, TextAlignmentOptions.MidlineLeft);
        ConfigureText(result.itemText, font, 22f, TextAlignmentOptions.MidlineLeft);

        Image background = dropdownObject.GetComponent<Image>();
        if (background != null)
        {
            background.color = new Color(0.14f, 0.15f, 0.2f, 1f);
        }

        Image templateBackground = result.template != null ? result.template.GetComponent<Image>() : null;
        if (templateBackground != null)
        {
            templateBackground.color = new Color(0.1f, 0.11f, 0.15f, 1f);
        }

        Toggle itemToggle = result.template != null ? result.template.GetComponentInChildren<Toggle>(true) : null;
        if (itemToggle != null && itemToggle.targetGraphic is Image itemBackground)
        {
            itemBackground.color = new Color(0.14f, 0.15f, 0.2f, 1f);
        }

        dropdownObject.AddComponent<ResolutionDropdownSetting>();
        return result;
    }

    private static void ConfigureText(TMP_Text text, TMP_FontAsset font, float size, TextAlignmentOptions alignment)
    {
        if (text == null)
        {
            return;
        }

        if (font != null)
        {
            text.font = font;
        }

        text.fontSize = size;
        text.color = Color.white;
        text.alignment = alignment;
    }
#endif
}
