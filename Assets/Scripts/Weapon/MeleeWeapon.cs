using System.Collections;
using UnityEngine;

public enum MeleeAttackType
{
    Thrust, // 突刺型
    Slash   // 挥砍型
}

public class MeleeWeapon : WeaponBase
{
    [Header("近战属性")]
    public MeleeAttackType attackType = MeleeAttackType.Thrust;
    public float attackDuration = 0.2f; // 攻击动作的持续时间

    [Header("突刺弹簧手感参数")]
    public float chargeDistance = 0.4f;     // 攻击前向后蓄力的距离
    public float bungeeOvershoot = 0.8f;    // 超过最大射程的“蹦极”拉伸过冲距离
    [Space]
    [Tooltip("各阶段时间占比 (总和必须 <= 1)")]
    public float timeToCharge = 0.15f;      // 向后蓄力阶段所需时间比例
    public float timeToThrust = 0.2f;       // 猛烈刺出并过冲阶段所需时间比例
    public float timeToRebound = 0.15f;     // 从过冲点弹回原射程时间比例
    public float timeToHold = 0.3f;         // 停顿时间比例
                                            // 剩下的时间全用于快速收回

    [Header("挥砍设置")]
    public float slashAngle = 120f;     // 挥砍划过的总角度

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private void Start()
    {
        // 记录武器装配在槽位上的初始本地位置和旋转，方便攻击结束后复原
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    protected override void Attack()
    {
        // 如果上一次攻击动画还没播完，则跳过
        if (!isAttacking)
        {
            base.Attack();
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        float timer = 0f;

        if (attackType == MeleeAttackType.Thrust)
        {
            // === 突刺逻辑 ===
            Vector3 chargePosition = originalLocalPosition - Vector3.right * chargeDistance;
            Vector3 overshootPosition = originalLocalPosition + Vector3.right * (attackRange + bungeeOvershoot);
            Vector3 targetPosition = originalLocalPosition + Vector3.right * attackRange;

            // 计算各阶段的实际结束时间点
            float phase1 = attackDuration * timeToCharge;
            float phase2 = phase1 + (attackDuration * timeToThrust);
            float phase3 = phase2 + (attackDuration * timeToRebound);
            float phase4 = phase3 + (attackDuration * timeToHold);

            while (timer < attackDuration)
            {
                timer += Time.deltaTime;

                if (timer <= phase1)
                {
                    // 阶段1：向后蓄力 (使用 SmoothStep 做出渐进发力的感觉)
                    float t = timer / phase1;
                    transform.localPosition = Vector3.Lerp(originalLocalPosition, chargePosition, Mathf.SmoothStep(0f, 1f, t));
                }
                else if (timer <= phase2)
                {
                    // 阶段2：猛烈刺出并过冲
                    float t = (timer - phase1) / (phase2 - phase1);
                    transform.localPosition = Vector3.Lerp(chargePosition, overshootPosition, t);
                }
                else if (timer <= phase3)
                {
                    // 阶段3：从过冲点弹回最大射程
                    float t = (timer - phase2) / (phase3 - phase2);
                    transform.localPosition = Vector3.Lerp(overshootPosition, targetPosition, t);
                }
                else if (timer <= phase4)
                {
                    // 阶段4：在最大射程处短暂停顿
                    transform.localPosition = targetPosition;
                }
                else
                {
                    // 阶段5：快速收回 (使用剩下的时间)
                    float t = (timer - phase4) / (attackDuration - phase4);
                    transform.localPosition = Vector3.Lerp(targetPosition, originalLocalPosition, Mathf.SmoothStep(0f, 1f, t));
                }
                yield return null;
            }
        }
        else if (attackType == MeleeAttackType.Slash)
        {
            // === 挥砍逻辑 ===
            // 2D中，Z轴正方向是逆时针，负方向是顺时针。顺时针挥砍就是从正角度砍向负角度
            Quaternion startRotation = originalLocalRotation * Quaternion.Euler(0, 0, slashAngle / 2f);
            Quaternion endRotation = originalLocalRotation * Quaternion.Euler(0, 0, -slashAngle / 2f);

            while (timer < attackDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / attackDuration;

                transform.localRotation = Quaternion.Lerp(startRotation, endRotation, progress);
                yield return null;
            }
        }

        // 攻击动作结束，确保强行归位，避免误差累积
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        isAttacking = false;
    }
}