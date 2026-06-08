using UnityEngine;

public class Melee_TrustWeapon : WeaponBase
{
    [Header("突刺蓄力设置")]
    public float chargeDuration = 0.13f;
    public float chargeDistance = 0.3f;

    [Header("突刺运动学设置")]
    public float thrustInDuration = 0.11f;
    public float thrustOutDuration = 0.2f;

    [Header("范围(Range)转化系数")]
    public float rangeToDistanceRatio = 0.02f;
    public float rangeToHoldTimeRatio = 0.0015f;

    private Vector3 originalLocalPosition;

    private enum ThrustState { None, Charging, Thrusting, Holding, Retracting }
    private ThrustState currentState = ThrustState.None;
    private float stateTimer = 0f;

    private float holdDuration;
    private Vector3 chargePosition;
    private Vector3 targetPosition;

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
            ProcessThrustStateMachine();
        }
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            base.Attack();

            isAttacking = true;
            currentState = ThrustState.Charging;
            stateTimer = 0f;

            float actualMeleeRange = attackRange * 0.5f;
            float thrustDistance = actualMeleeRange * rangeToDistanceRatio;
            holdDuration = actualMeleeRange * rangeToHoldTimeRatio;

            Vector3 localAimDirection = transform.parent != null ?
                transform.parent.InverseTransformDirection(aimDirection) : aimDirection;

            chargePosition = originalLocalPosition - localAimDirection * chargeDistance;
            targetPosition = originalLocalPosition + localAimDirection * thrustDistance;
        }
    }

    private void ProcessThrustStateMachine()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case ThrustState.Charging:
                float chargeProgress = stateTimer / chargeDuration;
                if (chargeProgress >= 1f)
                {
                    chargeProgress = 1f;
                    currentState = ThrustState.Thrusting;
                    stateTimer = 0f;
                }
                transform.localPosition = Vector3.Lerp(originalLocalPosition, chargePosition, Mathf.SmoothStep(0f, 1f, chargeProgress));
                break;

            case ThrustState.Thrusting:
                float thrustProgress = stateTimer / thrustInDuration;
                if (thrustProgress >= 1f)
                {
                    thrustProgress = 1f;
                    currentState = ThrustState.Holding;
                    stateTimer = 0f;
                }
                transform.localPosition = Vector3.Lerp(chargePosition, targetPosition, Mathf.SmoothStep(0f, 1f, thrustProgress));
                break;

            case ThrustState.Holding:
                if (stateTimer >= holdDuration)
                {
                    currentState = ThrustState.Retracting;
                    stateTimer = 0f;
                }
                break;

            case ThrustState.Retracting:
                float retractProgress = stateTimer / thrustOutDuration;
                if (retractProgress >= 1f)
                {
                    isAttacking = false;
                    currentState = ThrustState.None;
                    transform.localPosition = originalLocalPosition;
                }
                else
                {
                    transform.localPosition = Vector3.Lerp(targetPosition, originalLocalPosition, Mathf.SmoothStep(0f, 1f, retractProgress));
                }
                break;
        }
    }
}
