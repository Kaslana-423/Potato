using UnityEngine;
using UnityEngine.Pool; // 引入对象池

public class RangedWeapon : WeaponBase
{
    [Header("远程属性")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;
    public float rangeToDistanceRatio = 0.01f;

    // 重构：建立子弹对象池
    private ObjectPool<GameObject> bulletPool;

    protected override void Awake()
    {
        base.Awake();
        // 初始化对象池
        bulletPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    protected override void Attack()
    {
        base.Attack();
        Shoot();
    }

    protected override float GetFlatDamageBonus(PlayerStats stats)
    {
        return stats != null ? stats.RangedDamage : 0f;
    }

    private void Shoot()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        // 从池中拿取子弹，而不是 Instantiate
        GameObject bullet = bulletPool.Get();
        bullet.transform.position = spawnPos;
        bullet.transform.rotation = transform.rotation;

        ProjectileDamageOnHit projectileDamage = bullet.GetComponent<ProjectileDamageOnHit>();
        if (projectileDamage == null)
        {
            projectileDamage = bullet.AddComponent<ProjectileDamageOnHit>();
        }

        int projectileVersion = projectileDamage.Configure(GetAttackDamage(), bulletPool.Release);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.right * bulletSpeed;
        }

        float actualDistance = attackRange * rangeToDistanceRatio;
        float lifeTime = actualDistance / bulletSpeed;

        // 重构：这里不再使用 Destroy，而是利用自己封装的组件/协程将子弹放回池子
        // 为了不加新功能，这里用 Invoke 模拟子弹生命周期结束回收。实战建议写在子弹脚本里。
        StartCoroutine(ReturnBulletToPool(projectileDamage, projectileVersion, lifeTime));
    }

    private System.Collections.IEnumerator ReturnBulletToPool(ProjectileDamageOnHit projectile, int projectileVersion, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (projectile != null && projectile.gameObject.activeSelf)
        {
            projectile.Expire(projectileVersion);
        }
    }
}
