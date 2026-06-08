using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Melee_SlashWeapon : WeaponBase
{
    [Header("Brotato Slash Feel")]
    [Tooltip("Total slash duration preview, calculated from the four phase durations below.")]
    [Min(0.01f)] public float atkDuration = 0.3f;

    [Header("Slash Phase Timing")]
    [Min(0.001f)] public float windupDuration = 0.06f;
    [Min(0.001f)] public float sweepInDuration = 0.075f;
    [Min(0.001f)] public float sweepOutDuration = 0.075f;
    [Min(0.001f)] public float returnDuration = 0.09f;

    [Header("Slash Path")]
    [Min(0f)] public float recoilDistance = 0.2f;
    [Tooltip("Side offset = attackRange * sideRangeMultiplier.")]
    public float sideRangeMultiplier = 0.5f;
    [Tooltip("Weapon rotation angle from one side of the slash to the other.")]
    [Range(0f, 220f)] public float sweepAngle = 162f;
    [Tooltip("Forward center point distance = attackRange * centerDistanceMultiplier.")]
    [Range(0f, 1.5f)] public float centerDistanceMultiplier = 0.75f;
    [Tooltip("0 keeps straight segments. 1 uses the full curved path.")]
    [Range(0f, 1f)] public float pathCurveAmount = 0.75f;

    [Header("Scene Preview")]
    public bool showSlashGizmos = true;
    public bool showAlternateSlashGizmos = true;
    [Tooltip("Preview aim angle in Scene view when not playing. 0 is right, 90 is up.")]
    public float previewAimAngle = 0f;
    [Range(2, 64)] public int gizmoSamplesPerSegment = 24;
    [Min(0.005f)] public float gizmoPointRadius = 0.04f;
    [Min(0f)] public float gizmoRotationMarkerLength = 0.35f;
    public Color nextSlashGizmoColor = new Color(1f, 0.25f, 0.1f, 1f);
    public Color alternateSlashGizmoColor = new Color(0.1f, 0.65f, 1f, 0.55f);

    private Vector3 originalLocalPosition;

    private enum SlashState { None, Windup, SweepIn, SweepOut, Return }
    private SlashState currentState = SlashState.None;
    private float stateTimer = 0f;

    private float activeWindupDuration;
    private float activeSweepInDuration;
    private float activeSweepOutDuration;
    private float activeReturnDuration;

    private Vector3 pStart;
    private Vector3 pWindup;
    private Vector3 pCenter;
    private Vector3 pEnd;

    private float angleWindup;
    private float angleCenter;
    private float angleEnd;

    private bool flipAttackDirection = false;

    private struct SlashPath
    {
        public Vector3 start;
        public Vector3 windup;
        public Vector3 center;
        public Vector3 end;
        public float windupAngle;
        public float centerAngle;
        public float endAngle;
    }

    protected override void Awake()
    {
        base.Awake();
        originalLocalPosition = transform.localPosition;
        EnsureDamageHitboxesInChildren();
    }

    protected override void Update()
    {
        base.Update();
        if (isAttacking)
        {
            ProcessSlashStateMachine();
        }
        else
        {
            transform.localPosition = originalLocalPosition;
        }
    }

    protected override void Attack()
    {
        if (isAttacking) return;

        base.Attack();
        isAttacking = true;
        currentState = SlashState.Windup;
        stateTimer = 0f;

        CachePhaseDurations();
        CacheSlashPath(BuildSlashPath(aimDirection, flipAttackDirection));

        flipAttackDirection = !flipAttackDirection;
    }

    private void CachePhaseDurations()
    {
        activeWindupDuration = Mathf.Max(0.0001f, windupDuration);
        activeSweepInDuration = Mathf.Max(0.0001f, sweepInDuration);
        activeSweepOutDuration = Mathf.Max(0.0001f, sweepOutDuration);
        activeReturnDuration = Mathf.Max(0.0001f, returnDuration);
        atkDuration = GetTotalPhaseDuration();
    }

    private SlashPath BuildSlashPath(Vector3 worldAimDirection, bool flipped)
    {
        Vector3 worldAim = worldAimDirection.sqrMagnitude > 0.0001f
            ? worldAimDirection.normalized
            : Vector3.right;

        Vector3 localAim = transform.parent != null
            ? transform.parent.InverseTransformDirection(worldAim)
            : worldAim;
        localAim.z = 0f;
        localAim = localAim.sqrMagnitude > 0.0001f ? localAim.normalized : Vector3.right;

        Vector3 localPerp = new Vector3(-localAim.y, localAim.x, 0f).normalized;

        float sideRange = attackRange * sideRangeMultiplier;
        float sideA = flipped ? -sideRange : sideRange;
        float sideB = flipped ? sideRange : -sideRange;
        float angleA = flipped ? -sweepAngle : sweepAngle;
        float angleB = flipped ? sweepAngle : -sweepAngle;
        float baseAimAngle = Mathf.Atan2(worldAim.y, worldAim.x) * Mathf.Rad2Deg;

        Vector3 start = GetRestLocalPosition();

        return new SlashPath
        {
            start = start,
            windup = start - localAim * recoilDistance + localPerp * sideA,
            center = start + localAim * attackRange * centerDistanceMultiplier,
            end = start - localAim * recoilDistance + localPerp * sideB,
            windupAngle = baseAimAngle + angleA,
            centerAngle = baseAimAngle,
            endAngle = baseAimAngle + angleB
        };
    }

    private void CacheSlashPath(SlashPath path)
    {
        pStart = path.start;
        pWindup = path.windup;
        pCenter = path.center;
        pEnd = path.end;
        angleWindup = path.windupAngle;
        angleCenter = path.centerAngle;
        angleEnd = path.endAngle;
    }

    private Vector3 GetRestLocalPosition()
    {
        if (Application.isPlaying)
        {
            return originalLocalPosition;
        }

        return transform.localPosition;
    }

    private void ProcessSlashStateMachine()
    {
        stateTimer += Time.deltaTime;
        float progress;

        switch (currentState)
        {
            case SlashState.Windup:
                progress = stateTimer / activeWindupDuration;
                if (progress >= 1f) { progress = 1f; SwitchState(SlashState.SweepIn); }

                float windupT = EaseOutExpo(progress);
                transform.localPosition = EvaluateSlashSegment(SlashState.Windup, windupT);
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(angleCenter, angleWindup, windupT) + visualRotationOffset);
                break;

            case SlashState.SweepIn:
                progress = stateTimer / activeSweepInDuration;
                if (progress >= 1f) { progress = 1f; SwitchState(SlashState.SweepOut); }

                transform.localPosition = EvaluateSlashSegment(SlashState.SweepIn, progress);
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(angleWindup, angleCenter, progress) + visualRotationOffset);
                break;

            case SlashState.SweepOut:
                progress = stateTimer / activeSweepOutDuration;
                if (progress >= 1f) { progress = 1f; SwitchState(SlashState.Return); }

                transform.localPosition = EvaluateSlashSegment(SlashState.SweepOut, progress);
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(angleCenter, angleEnd, progress) + visualRotationOffset);
                break;

            case SlashState.Return:
                progress = stateTimer / activeReturnDuration;
                if (progress >= 1f)
                {
                    isAttacking = false;
                    currentState = SlashState.None;
                    transform.localPosition = pStart;
                }
                else
                {
                    float returnT = EaseOutExpo(progress);
                    transform.localPosition = EvaluateSlashSegment(SlashState.Return, returnT);

                    float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(angleEnd, targetAngle, returnT) + visualRotationOffset);
                }
                break;
        }
    }

    private void SwitchState(SlashState newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }

    private Vector3 EvaluateSlashSegment(SlashState state, float t)
    {
        return EvaluateSlashSegment(new SlashPath
        {
            start = pStart,
            windup = pWindup,
            center = pCenter,
            end = pEnd
        }, state, t);
    }

    private Vector3 EvaluateSlashSegment(SlashPath path, SlashState state, float t)
    {
        t = Mathf.Clamp01(t);

        Vector3 curved;
        Vector3 straight;

        switch (state)
        {
            case SlashState.Windup:
                curved = CatmullRom(path.start, path.start, path.windup, path.center, t);
                straight = Vector3.Lerp(path.start, path.windup, t);
                break;

            case SlashState.SweepIn:
                curved = CatmullRom(path.start, path.windup, path.center, path.end, t);
                straight = Vector3.Lerp(path.windup, path.center, t);
                break;

            case SlashState.SweepOut:
                curved = CatmullRom(path.windup, path.center, path.end, path.start, t);
                straight = Vector3.Lerp(path.center, path.end, t);
                break;

            case SlashState.Return:
                curved = CatmullRom(path.center, path.end, path.start, path.start, t);
                straight = Vector3.Lerp(path.end, path.start, t);
                break;

            default:
                return path.start;
        }

        return Vector3.Lerp(straight, curved, pathCurveAmount);
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            2f * p1 +
            (p2 - p0) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (3f * p1 - p0 - 3f * p2 + p3) * t3
        );
    }

    private float EaseOutExpo(float x)
    {
        return x >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
    }

    private float GetTotalPhaseDuration()
    {
        return windupDuration + sweepInDuration + sweepOutDuration + returnDuration;
    }

    private void OnValidate()
    {
        windupDuration = Mathf.Max(0.001f, windupDuration);
        sweepInDuration = Mathf.Max(0.001f, sweepInDuration);
        sweepOutDuration = Mathf.Max(0.001f, sweepOutDuration);
        returnDuration = Mathf.Max(0.001f, returnDuration);
        atkDuration = Mathf.Max(0.01f, GetTotalPhaseDuration());
        recoilDistance = Mathf.Max(0f, recoilDistance);
        pathCurveAmount = Mathf.Clamp01(pathCurveAmount);
        gizmoSamplesPerSegment = Mathf.Max(2, gizmoSamplesPerSegment);
        gizmoPointRadius = Mathf.Max(0.005f, gizmoPointRadius);
        gizmoRotationMarkerLength = Mathf.Max(0f, gizmoRotationMarkerLength);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showSlashGizmos) return;

        Vector3 previewAim = GetGizmoAimDirection();
        DrawSlashPathGizmos(BuildSlashPath(previewAim, flipAttackDirection), nextSlashGizmoColor, true);

        if (showAlternateSlashGizmos)
        {
            DrawSlashPathGizmos(BuildSlashPath(previewAim, !flipAttackDirection), alternateSlashGizmoColor, false);
        }
    }

    private Vector3 GetGizmoAimDirection()
    {
        if (Application.isPlaying && aimDirection.sqrMagnitude > 0.0001f)
        {
            return aimDirection.normalized;
        }

        float radians = previewAimAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);
    }

    private void DrawSlashPathGizmos(SlashPath path, Color color, bool drawLabels)
    {
        DrawSampledCurve(path, SlashState.Windup, color);
        DrawSampledCurve(path, SlashState.SweepIn, color);
        DrawSampledCurve(path, SlashState.SweepOut, color);
        DrawSampledCurve(path, SlashState.Return, WithAlpha(color, color.a * 0.35f));

        DrawPoint(path.start, color);
        DrawPoint(path.windup, color);
        DrawPoint(path.center, color);
        DrawPoint(path.end, color);

        DrawRotationMarker(path.windup, path.windupAngle, color);
        DrawRotationMarker(path.center, path.centerAngle, color);
        DrawRotationMarker(path.end, path.endAngle, color);

#if UNITY_EDITOR
        if (drawLabels)
        {
            Handles.color = color;
            Handles.Label(ToWorldPoint(path.start), "Start");
            Handles.Label(ToWorldPoint(path.windup), "Windup");
            Handles.Label(ToWorldPoint(path.center), "Center");
            Handles.Label(ToWorldPoint(path.end), "End");
        }
#endif
    }

    private void DrawSampledCurve(SlashPath path, SlashState state, Color color)
    {
        Gizmos.color = color;
        Vector3 previous = ToWorldPoint(EvaluateSlashSegment(path, state, 0f));

        for (int i = 1; i <= gizmoSamplesPerSegment; i++)
        {
            float t = i / (float)gizmoSamplesPerSegment;
            Vector3 current = ToWorldPoint(EvaluateSlashSegment(path, state, t));
            Gizmos.DrawLine(previous, current);
            previous = current;
        }
    }

    private void DrawPoint(Vector3 localPoint, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(ToWorldPoint(localPoint), Mathf.Max(0.005f, gizmoPointRadius));
    }

    private void DrawRotationMarker(Vector3 localPoint, float angle, Color color)
    {
        if (gizmoRotationMarkerLength <= 0f) return;

        Vector3 worldPoint = ToWorldPoint(localPoint);
        Vector3 direction = Quaternion.Euler(0f, 0f, angle + visualRotationOffset) * Vector3.right;

        Gizmos.color = WithAlpha(color, color.a * 0.8f);
        Gizmos.DrawLine(worldPoint, worldPoint + direction * gizmoRotationMarkerLength);
    }

    private Vector3 ToWorldPoint(Vector3 localPoint)
    {
        return transform.parent != null
            ? transform.parent.TransformPoint(localPoint)
            : localPoint;
    }

    private Color WithAlpha(Color color, float alpha)
    {
        color.a = Mathf.Clamp01(alpha);
        return color;
    }
}
