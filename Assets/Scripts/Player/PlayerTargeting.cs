using UnityEngine;

/// <summary>
/// 中央索敌雷达。
/// 剥离武器自身的物理查询，统一管理同屏最近的敌人。
/// </summary>
public class PlayerTargeting : MonoBehaviour
{
    [Header("索敌设置")]
    public float globalDetectRadius = 20f;
    public LayerMask enemyLayer;

    // 暴露给所有武器的只读属性
    public Transform ClosestEnemy { get; private set; }
    public Vector3 AimDirection { get; private set; } = Vector3.right;

    // 预分配内存，大小决定了索敌的极限数量
    private Collider2D[] enemyBuffer = new Collider2D[50];

    void Update()
    {
        FindClosestEnemy();
    }

    private void FindClosestEnemy()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, globalDetectRadius, enemyBuffer, enemyLayer);

        if (count == 0)
        {
            ClosestEnemy = null;
            return;
        }

        float minDistanceSqr = Mathf.Infinity;
        Transform target = null;
        Vector3 currentPos = transform.position;

        for (int i = 0; i < count; i++)
        {
            float dSqr = (enemyBuffer[i].transform.position - currentPos).sqrMagnitude;
            if (dSqr < minDistanceSqr)
            {
                minDistanceSqr = dSqr;
                target = enemyBuffer[i].transform;
            }
        }

        ClosestEnemy = target;
        if (ClosestEnemy != null)
        {
            AimDirection = (ClosestEnemy.position - transform.position).normalized;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, globalDetectRadius);
    }
}