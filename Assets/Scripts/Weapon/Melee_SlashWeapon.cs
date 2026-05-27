using UnityEngine;

public class Melee_SlashWeapon : WeaponBase
{
    [Header("Brotato 原汁原味挥砍设置")]
    public float atkDuration = 0.3f;        // 完整攻击耗时
    public float recoilDistance = 0.2f;     // 起手向后蓄力的距离 (对应源码的 recoil)
    public float sideRangeMultiplier = 0.5f; // 侧向偏移系数 (对应源码的 side_range)

    private Vector3 originalLocalPosition;

    // 严丝合缝对标 Godot 源码的 4 个阶段
    private enum SlashState { None, Windup, SweepIn, SweepOut, Return }
    private SlashState currentState = SlashState.None;
    private float stateTimer = 0f;

    // 动态计算的阶段耗时 (对标源码的时间切分)
    private float recoilDuration;
    private float sweepHalfDuration;
    private float backDuration;

    // 核心位移点与旋转角度
    private Vector3 pStart;     // 初始原点
    private Vector3 pWindup;    // 举刀点 (向后、向侧边)
    private Vector3 pCenter;    // 劈砍顶点 (正前方 0.75 倍距离)
    private Vector3 pEnd;       // 挥砍结束点 (向后、向另一侧)

    private float angleWindup;
    private float angleCenter;
    private float angleEnd;

    // 控制左右交替挥砍
    private bool flipAttackDirection = false;

    protected override void Awake()
    {
        base.Awake();
        originalLocalPosition = transform.localPosition;
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
        if (!isAttacking)
        {
            base.Attack();
            isAttacking = true;
            currentState = SlashState.Windup;
            stateTimer = 0f;

            // 1. 时间切分 (还原源码逻辑)
            recoilDuration = atkDuration * 0.2f;
            sweepHalfDuration = atkDuration * 0.25f;
            backDuration = atkDuration * 0.3f;

            // 2. 空间点位计算 (V字形轨迹)
            float atkDistance = attackRange;
            float sideRange = atkDistance * sideRangeMultiplier;
            float sweepAngle = 162f;

            // 保证翻转逻辑完全一致
            float sideA = flipAttackDirection ? -sideRange : sideRange;
            float sideB = flipAttackDirection ? sideRange : -sideRange;
            float angleA = flipAttackDirection ? -sweepAngle : sweepAngle;
            float angleB = flipAttackDirection ? sweepAngle : -sweepAngle;

            Vector3 logicalDir = transform.parent != null ?
                transform.parent.InverseTransformDirection(aimDirection) : aimDirection;

            // 【核心修复】：统一空间与旋转的数学符号
            // 在 Unity 2D 中，正角度代表逆时针（Counter-Clockwise）。
            // 所以我们需要一个严格逆时针旋转 90 度的垂直向量，即 (-y, x) ，而不是顺时针的 (y, -x)
            // 这样 sideA 和 angleA 符号相同时，位移方向和旋转方向才会完全同步
            Vector3 logicalPerp = new Vector3(-logicalDir.y, logicalDir.x, 0f).normalized;

            // 计算四个关键节点的局部坐标
            pStart = originalLocalPosition;
            // 使用修正后的严格逆时针向量
            pWindup = pStart - logicalDir * recoilDistance + logicalPerp * sideA;
            pCenter = pStart + logicalDir * atkDistance * 0.75f;
            pEnd = pStart - logicalDir * recoilDistance + logicalPerp * sideB;

            float baseAimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            angleWindup = baseAimAngle + angleA;
            angleCenter = baseAimAngle;
            angleEnd = baseAimAngle + angleB;

            // 每次挥击交替方向
            flipAttackDirection = !flipAttackDirection;
        }
    }

    private void ProcessSlashStateMachine()
    {
        stateTimer += Time.deltaTime;
        float progress;

        switch (currentState)
        {
            case SlashState.Windup: // 第一阶段：向后蓄力举刀
                progress = stateTimer / recoilDuration;
                if (progress >= 1f) { progress = 1f; SwitchState(SlashState.SweepIn); }

                float windupT = EaseOutExpo(progress);
                transform.localPosition = Vector3.Lerp(pStart, pWindup, windupT);

                // 【核心修复】：使用纯 Mathf.Lerp，强迫引擎按死数值插值，绝对禁止转圈
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(angleCenter, angleWindup, windupT) + visualRotationOffset);
                break;

            case SlashState.SweepIn: // 第二阶段：劈向中心极远点
                progress = stateTimer / sweepHalfDuration;
                if (progress >= 1f) { progress = 1f; SwitchState(SlashState.SweepOut); }

                transform.localPosition = Vector3.Lerp(pWindup, pCenter, progress);
                // 【核心修复】：同上，使用纯 Lerp
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(angleWindup, angleCenter, progress) + visualRotationOffset);

                // (此处放判定伤害的代码，如果需要的话)
                break;

            case SlashState.SweepOut: // 第三阶段：划拉到另一侧
                progress = stateTimer / sweepHalfDuration;
                if (progress >= 1f) { progress = 1f; SwitchState(SlashState.Return); }

                transform.localPosition = Vector3.Lerp(pCenter, pEnd, progress);
                // 【核心修复】：同上，使用纯 Lerp
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(angleCenter, angleEnd, progress) + visualRotationOffset);
                break;

            case SlashState.Return: // 第四阶段：收刀回原点
                progress = stateTimer / backDuration;
                if (progress >= 1f)
                {
                    isAttacking = false;
                    currentState = SlashState.None;
                    transform.localPosition = pStart;
                }
                else
                {
                    float returnT = EaseOutExpo(progress);
                    transform.localPosition = Vector3.Lerp(pEnd, pStart, returnT);

                    // 【唯独这里保留 LerpAngle】：因为 targetAngle 是实时 Atan2 算出来的，可能会跨越 -180/180 的突变边界
                    float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(angleEnd, targetAngle, returnT) + visualRotationOffset);
                }
                break;
        }
    }

    private void SwitchState(SlashState newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }

    /// <summary>
    /// 模拟 Godot 中的 Tween.TRANS_EXPO + Tween.EASE_OUT
    /// 极其凌厉的缓动曲线：起步瞬间爆发，随后极慢平滑停止
    /// </summary>
    private float EaseOutExpo(float x)
    {
        return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
    }
}