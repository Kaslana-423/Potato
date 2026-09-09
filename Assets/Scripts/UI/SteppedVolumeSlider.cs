using UnityEngine;
using UnityEngine.UI;

public static class SteppedVolumeSlider
{
    public const int StepCount = 10;

    public static void Configure(Slider slider)
    {
        if (slider == null)
        {
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = StepCount;
        slider.wholeNumbers = true;
        slider.direction = Slider.Direction.LeftToRight;
    }

    public static void SetNormalizedValueWithoutNotify(Slider slider, float normalizedValue)
    {
        if (slider == null)
        {
            return;
        }

        slider.SetValueWithoutNotify(SnapNormalizedValue(normalizedValue) * StepCount);
    }

    public static float ToNormalizedValue(float sliderValue)
    {
        return Mathf.Clamp(Mathf.Round(sliderValue), 0f, StepCount) / StepCount;
    }

    public static float SnapNormalizedValue(float normalizedValue)
    {
        return Mathf.Round(Mathf.Clamp01(normalizedValue) * StepCount) / StepCount;
    }
}
