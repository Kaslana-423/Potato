using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerStatRowView : MonoBehaviour
{
    [SerializeField] private TMP_Text iconText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text valueText;

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
        if (iconText == null)
        {
            iconText = FindComponent<TMP_Text>("IconText", "Icon");
        }

        if (iconImage == null)
        {
            iconImage = FindComponent<Image>("IconImage");
        }

        if (nameText == null)
        {
            nameText = FindComponent<TMP_Text>("NameText", "Name");
        }

        if (valueText == null)
        {
            valueText = FindComponent<TMP_Text>("ValueText", "Value");
        }
    }

    public void Bind(PlayerStatDisplayEntry entry)
    {
        AutoBindReferences();

        if (iconText != null)
        {
            iconText.text = entry.IconText;
            iconText.color = entry.IconColor;
        }

        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(entry.IconSprite != null);
            iconImage.sprite = entry.IconSprite;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
        }

        if (nameText != null)
        {
            nameText.text = entry.DisplayName;
            nameText.color = entry.NameColor;
        }

        if (valueText != null)
        {
            valueText.text = entry.ValueText;
            valueText.color = entry.ValueColor;
        }
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

    private T FindComponent<T>(params string[] names) where T : Component
    {
        Transform child = FindDescendant(names);
        return child != null ? child.GetComponent<T>() : null;
    }
}
