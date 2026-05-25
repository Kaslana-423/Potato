using UnityEngine;

public class RangedWeapon : WeaponBase
{
    [Header("远程属性")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;

    [Header("数值转化")]
    [Tooltip("将基础射程数值(如350)转化为实际飞行距离(米)的系数，需与近战武器保持统一")]
    public float rangeToDistanceRatio = 0.01f;

    protected override void Attack()
    {
        base.Attack();
        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("远程武器没有配置子弹预制体！");
            return;
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        // 生成子弹
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, transform.rotation);

        // 优化1：绝对不要在战斗代码中 AddComponent！
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 赋予速度
            rb.velocity = transform.right * bulletSpeed;
        }
        else
        {
            Debug.LogError("严重错误：子弹预制体上缺少 Rigidbody2D 组件，请在 Prefab 面板中提前挂载！");
        }

        // 优化2：统一 Range 的计算逻辑
        float actualDistance = attackRange * rangeToDistanceRatio;

        // 优化3：计算存活时间 (时间 = 实际物理距离 / 速度)
        float lifeTime = actualDistance / bulletSpeed;
        Destroy(bullet, lifeTime);
    }
}