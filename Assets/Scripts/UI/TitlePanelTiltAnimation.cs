using UnityEngine;

[DisallowMultipleComponent]
public sealed class TitlePanelTiltAnimation : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationOffset = 3f;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float initialDelay;
    [SerializeField, Min(0.01f)] private float originalHoldDuration = 0.45f;
    [SerializeField, Min(0.01f)] private float rotatedHoldDuration = 0.16f;

    private Quaternion originalLocalRotation;
    private float elapsedTime;
    private float initialDelayRemaining;
    private float configuredInitialDelay;
    private bool delayClockStarted;
    private bool showingRotatedState;
    private bool rotationCaptured;

    private void Awake()
    {
        CaptureOriginalRotation();
    }

    private void OnEnable()
    {
        if (!rotationCaptured)
        {
            CaptureOriginalRotation();
        }

        RestartAnimation();
    }

    private void Update()
    {
        if (!Mathf.Approximately(configuredInitialDelay, Mathf.Max(0f, initialDelay)))
        {
            RestartAnimation();
            return;
        }

        if (!delayClockStarted)
        {
            delayClockStarted = true;
            return;
        }

        float deltaTime = Mathf.Min(Time.unscaledDeltaTime, 0.1f);
        if (initialDelayRemaining > 0f)
        {
            initialDelayRemaining -= deltaTime;
            if (initialDelayRemaining > 0f)
            {
                return;
            }

            deltaTime = -initialDelayRemaining;
            initialDelayRemaining = 0f;
        }

        elapsedTime += deltaTime;
        float stateDuration = showingRotatedState
            ? Mathf.Max(0.01f, rotatedHoldDuration)
            : Mathf.Max(0.01f, originalHoldDuration);
        if (elapsedTime < stateDuration)
        {
            return;
        }

        elapsedTime = 0f;
        showingRotatedState = !showingRotatedState;
        ApplyCurrentState();
    }

    private void OnDisable()
    {
        if (rotationCaptured)
        {
            transform.localRotation = originalLocalRotation;
        }
    }

    private void CaptureOriginalRotation()
    {
        originalLocalRotation = transform.localRotation;
        rotationCaptured = true;
    }

    private void RestartAnimation()
    {
        configuredInitialDelay = Mathf.Max(0f, initialDelay);
        initialDelayRemaining = configuredInitialDelay;
        delayClockStarted = false;
        elapsedTime = 0f;
        showingRotatedState = false;
        ApplyCurrentState();
    }

    private void ApplyCurrentState()
    {
        transform.localRotation = showingRotatedState
            ? originalLocalRotation * Quaternion.AngleAxis(rotationOffset, Vector3.forward)
            : originalLocalRotation;
    }
}
