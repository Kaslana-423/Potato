using System.Collections;
using UnityEngine;

public class Melee_SlashWeapon : WeaponBase
{
    [Header("挥砍运动学设置")]
    public float slashDuration = 0.2f;      // 挥砍过程耗时
    public float slashAngle = 120f;         // 挥砍划过的总角度

    [Header("范围(Range)转化系数")]
    [Tooltip("范围越大，挥砍时武器向外延展的甩出距离越远")]
    public float rangeToDistanceRatio = 0.01f;
    [Tooltip("范围越大，挥砍结束后的停留/僵直时间越久")]
    public float rangeToHoldTimeRatio = 0.001f;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private void Start()
    {
        // 挥砍现在不仅需要记录旋转，也需要记录初始位置，用于计算向外的位移
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            base.Attack();
            StartCoroutine(PerformSlashAttack());
        }
    }

    private IEnumerator PerformSlashAttack()
    {
        isAttacking = true;

        // 1. 计算范围带来的动态属性
        float actualMeleeRange = attackRange * 0.5f;
        float holdDuration = actualMeleeRange * rangeToHoldTimeRatio;
        // 计算挥砍时向外甩出的最大距离
        float maxOutwardDistance = actualMeleeRange * rangeToDistanceRatio;

        float timer = 0f;
        Quaternion startRotation = originalLocalRotation * Quaternion.Euler(0, 0, slashAngle / 2f);
        Quaternion endRotation = originalLocalRotation * Quaternion.Euler(0, 0, -slashAngle / 2f);

        // ==========================================
        // 1. 挥砍动作 (包含动态向外位移)
        // ==========================================
        while (timer < slashDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / slashDuration;

            // 处理旋转
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, Mathf.SmoothStep(0f, 1f, progress));

            // 处理位移：使用 Sin 曲线绘制半月形弧线 (progress从0到1，Sin值从0 -> 1 -> 0)
            float currentOutwardOffset = maxOutwardDistance * Mathf.Sin(progress * Mathf.PI);

            // 核心数学：获取武器当前旋转状态下的“正前/正右方”
            // 假设你的武器贴图默认是朝向 Right (X轴正方向)，如果你的武器贴图朝上，请将 Vector3.right 改为 Vector3.up
            Vector3 outwardDirection = transform.localRotation * aimDirection.normalized; // 获取当前旋转状态下的武器朝向

            // 将武器沿着当前指向向外推
            transform.localPosition = originalLocalPosition + outwardDirection * currentOutwardOffset;

            yield return null;
        }

        // 确保挥砍结束时，位置和旋转完全卡准在结束点
        transform.localRotation = endRotation;
        transform.localPosition = originalLocalPosition;

        // ==========================================
        // 2. 动作后摇/停留 (硬直)
        // ==========================================
        if (holdDuration > 0f)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        // ==========================================
        // 3. 快速收回复位
        // ==========================================
        timer = 0f;
        float resetDuration = 0.05f;
        while (timer < resetDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(endRotation, originalLocalRotation, timer / resetDuration);
            yield return null;
        }

        // 结束复位
        transform.localRotation = originalLocalRotation;
        transform.localPosition = originalLocalPosition;
        isAttacking = false;
    }
}