using UnityEngine;

public enum WeaponRarity
{
    Common,    // 普通
    Rare,      // 稀有
    Epic,      // 史诗
    Legendary  // 传说
}

public abstract class WeaponBase : MonoBehaviour
{
    [Header("基础属性")]
    public string weaponName;
    public Sprite weaponIcon;
    [TextArea]
    public string description;
    public float attackPower = 10f;
    public float attackRange = 5f; // 统一的武器攻击范围（射程）
    public WeaponRarity rarity = WeaponRarity.Common;

    [Header("攻击设置")]
    public float attackCooldown = 1f; // 攻击间隔（秒）

    [Header("目标检测")]
    public LayerMask enemyLayer; // 性能优化：通过LayerMask只检测敌人所在的层级
    protected Vector3 aimDirection = Vector3.left; // 优化命名：明确表示“瞄准方向”

    protected float currentCooldown = 0f;
    protected bool isAttacking = false; // 攻击状态标志

    // 性能优化：预先分配好一块内存，避免每帧生成数组产生大量的垃圾回收(GC)
    // 数组大小决定了你一次最多能检测到几个敌人（这里设为20通常足够找最近的了）
    private Collider2D[] enemyBuffer = new Collider2D[20];

    protected virtual void Update()
    {
        // 1. 每帧更新瞄准方向 (更新数据)
        UpdateAimDirection();
        // 2. 处理武器的视觉旋转 (更新表现)
        UpdateWeaponRotation();

        // 如果动作正在执行中（如正在挥刀），则暂停冷却计时
        if (isAttacking) return;
        // 2. 冷却与攻击逻辑
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
        else
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        // 重置冷却时间
        currentCooldown = attackCooldown;
    }

    /// <summary>
    /// 使用零GC的物理检测方案寻找最近的敌人并更新 aimDirection
    /// </summary>
    private void UpdateAimDirection()
    {
        // 核心优化：OverlapCircleNonAlloc 不会像 OverlapCircleAll 那样每帧新建一个数组
        // 它会把找到的敌人塞进我们预先准备好的 enemyBuffer 数组里，并返回找到了几个
        int enemyCount = Physics2D.OverlapCircleNonAlloc(transform.position, attackRange, enemyBuffer, enemyLayer);

        // 如果范围内没有敌人，保持原方向，或者也可以归零，视你的业务需求而定
        if (enemyCount == 0) return;

        Transform closestEnemy = null;
        float minDistanceSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        // 注意：遍历的上限是 enemyCount，因为 buffer 里只有前 enemyCount 个元素是有效的
        for (int i = 0; i < enemyCount; i++)
        {
            Collider2D enemyCollider = enemyBuffer[i];

            // 使用 sqrMagnitude 比较距离，比 Vector3.Distance（需要开平方）快得多
            Vector3 directionToTarget = enemyCollider.transform.position - currentPos;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < minDistanceSqr)
            {
                minDistanceSqr = dSqrToTarget;
                closestEnemy = enemyCollider.transform;
            }
        }

        // 找到了最近的敌人，更新瞄准方向向量并归一化
        if (closestEnemy != null)
        {
            aimDirection = (closestEnemy.position - transform.position).normalized;
        }
    }
    /// <summary>
    /// 根据当前的 aimDirection 旋转武器
    /// </summary>
    protected virtual void UpdateWeaponRotation()
    {
        // Mathf.Atan2 接收 (y, x) 参数，返回对应的弧度值
        // 然后通过 Mathf.Rad2Deg 将弧度转换为 Unity Transform 所需的角度
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        // 在 2D 游戏中，物体的旋转通常只发生在 Z 轴上
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

    }

    // 可视化辅助：在Scene窗口画出攻击范围，方便你调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}