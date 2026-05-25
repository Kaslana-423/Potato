using System.Collections;
using UnityEngine;

public class ThrustWeapon : WeaponBase
{
    [Header("突刺蓄力设置")]
    [Tooltip("向后蓄力的阶段耗时")]
    public float chargeDuration = 0.13f;
    [Tooltip("向后蓄力的最大拉弓距离（米）")]
    public float chargeDistance = 0.3f;

    [Header("突刺运动学设置")]
    [Tooltip("从蓄力点刺向最远点阶段耗时")]
    public float thrustInDuration = 0.11f;
    [Tooltip("从最远点收回原位阶段耗时")]
    public float thrustOutDuration = 0.2f;

    [Header("范围(Range)转化系数")]
    [Tooltip("转化为实际位移距离(米)的系数")]
    public float rangeToDistanceRatio = 0.02f;
    [Tooltip("范围越大，攻击结束后的停留/僵直时间越久")]
    public float rangeToHoldTimeRatio = 0.0015f;

    private Vector3 originalLocalPosition;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            base.Attack();
            StartCoroutine(PerformThrustAttack());
        }
    }

    private IEnumerator PerformThrustAttack()
    {
        isAttacking = true;

        // ==========================================
        // 1. 计算基于 Range 的动态属性
        // ==========================================
        float actualMeleeRange = attackRange * 0.5f;
        float thrustDistance = actualMeleeRange * rangeToDistanceRatio;
        float holdDuration = actualMeleeRange * rangeToHoldTimeRatio;

        // 核心修复：把世界方向转为父物体的局部方向
        Vector3 localAimDirection = transform.parent.InverseTransformDirection(aimDirection);

        // 使用转换后的局部方向来计算位置
        Vector3 startPosition = originalLocalPosition;
        Vector3 chargePosition = originalLocalPosition - localAimDirection * chargeDistance;
        Vector3 targetPosition = originalLocalPosition + localAimDirection * thrustDistance;

        float timer = 0f;

        // ==========================================
        // 2. 蓄力阶段 (向后拉弓)
        // ==========================================
        while (timer < chargeDuration)
        {
            timer += Time.deltaTime;
            // Mathf.SmoothStep 让向后拉弓的过程带有一点渐进的阻尼感
            transform.localPosition = Vector3.Lerp(startPosition, chargePosition, Mathf.SmoothStep(0f, 1f, timer / chargeDuration));
            yield return null;
        }
        transform.localPosition = chargePosition; // 确保精准到达蓄力点

        // ==========================================
        // 3. 刺出阶段 (从蓄力点猛烈向前突刺)
        // ==========================================
        timer = 0f;
        while (timer < thrustInDuration)
        {
            timer += Time.deltaTime;
            // 注意：这里是从 chargePosition 插值到 targetPosition，跨度变大，视觉速度会更快
            transform.localPosition = Vector3.Lerp(chargePosition, targetPosition, Mathf.SmoothStep(0f, 1f, timer / thrustInDuration));
            yield return null;
        }
        transform.localPosition = targetPosition; // 确保完全到达最远点

        // ==========================================
        // 4. 远端停留 (受 Range 影响的僵直/穿透感)
        // ==========================================
        if (holdDuration > 0f)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        // ==========================================
        // 5. 收回阶段 (从最远点拔回初始位置)
        // ==========================================
        timer = 0f;
        while (timer < thrustOutDuration)
        {
            timer += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(targetPosition, startPosition, Mathf.SmoothStep(0f, 1f, timer / thrustOutDuration));
            yield return null;
        }

        // ==========================================
        // 6. 结束复位
        // ==========================================
        transform.localPosition = originalLocalPosition;
        isAttacking = false;
    }
}