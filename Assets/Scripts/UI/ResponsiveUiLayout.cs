using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shared rules for screen-space UI. The reference resolution is a design coordinate
/// system; Expand keeps that complete design area visible on every aspect ratio.
/// </summary>
public static class ResponsiveUiLayout
{
    public static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

    public static void ConfigureCanvasScaler(CanvasScaler scaler)
    {
        if (scaler == null)
        {
            return;
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = ReferenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
    }

    public static void SetNormalizedRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }
}
