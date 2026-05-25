using UnityEngine;

public class RangedWeapon : WeaponBase
{
    [Header("远程属性")]
    public GameObject bulletPrefab; // 子弹预制体（用简单的小圆球代替）
    public Transform firePoint;     // 子弹发射点（若为空，默认在武器中心生成）
    public float bulletSpeed = 15f; // 子弹飞行速度

    protected override void Attack()
    {
        // 调用基类方法重置冷却
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

        // 获取或添加刚体，赋予速度
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // 俯视角不需要受重力影响
        }

        // 假设武器朝向右方（X轴正方向），赋予速度
        rb.velocity = transform.right * bulletSpeed;

        // 根据武器射程计算子弹存活时间：时间 = 距离 / 速度
        // 这样子弹刚好在到达最大射程边界时消失，完美还原对远程武器的范围限制
        float lifeTime = attackRange / bulletSpeed;
        Destroy(bullet, lifeTime);
    }
}