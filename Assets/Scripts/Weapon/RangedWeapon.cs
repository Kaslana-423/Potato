using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RangedWeapon : WeaponBase
{
    [Header("远程属性")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;
    public float rangeToDistanceRatio = 0.01f;

    [Header("子弹对象池")]
    [SerializeField, Min(0)] private int poolDefaultCapacity = 20;
    [SerializeField, Min(1)] private int poolMaxSize = 100;
    [SerializeField, Min(0)] private int prewarmCount = 10;
    [SerializeField] private bool collectionChecks = true;
    [SerializeField] private Transform poolRoot;

    private ObjectPool<GameObject> bulletPool;
    private readonly HashSet<GameObject> activeBullets = new HashSet<GameObject>();

    public int ActiveBulletCount => activeBullets.Count;
    public int InactiveBulletCount => bulletPool != null ? bulletPool.CountInactive : 0;

    protected override void Awake()
    {
        base.Awake();
        EnsureBulletPool();
    }

    private void OnValidate()
    {
        poolDefaultCapacity = Mathf.Max(0, poolDefaultCapacity);
        poolMaxSize = Mathf.Max(1, poolMaxSize);
        prewarmCount = Mathf.Clamp(prewarmCount, 0, poolMaxSize);
        bulletSpeed = Mathf.Max(0.01f, bulletSpeed);
        rangeToDistanceRatio = Mathf.Max(0f, rangeToDistanceRatio);
    }

    private void OnDisable()
    {
        ReleaseAllActiveBullets();
    }

    private void OnDestroy()
    {
        ReleaseAllActiveBullets();
        bulletPool?.Clear();
        bulletPool = null;
    }

    private void EnsureBulletPool()
    {
        if (bulletPool != null || bulletPrefab == null)
        {
            return;
        }

        bulletPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: PrepareBulletOnGet,
            actionOnRelease: PrepareBulletOnRelease,
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: collectionChecks,
            defaultCapacity: poolDefaultCapacity,
            maxSize: poolMaxSize
        );

        int warmCount = Mathf.Min(prewarmCount, poolMaxSize);
        var warmedBullets = new List<GameObject>(warmCount);
        for (int index = 0; index < warmCount; index++)
        {
            warmedBullets.Add(bulletPool.Get());
        }

        foreach (GameObject bullet in warmedBullets)
        {
            bulletPool.Release(bullet);
        }
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
        EnsureBulletPool();
        if (bulletPool == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        // 从池中拿取子弹，而不是 Instantiate
        GameObject bullet = bulletPool.Get();
        activeBullets.Add(bullet);
        bullet.transform.position = spawnPos;
        bullet.transform.rotation = transform.rotation;

        ProjectileDamageOnHit projectileDamage = bullet.GetComponent<ProjectileDamageOnHit>();
        if (projectileDamage == null)
        {
            projectileDamage = bullet.AddComponent<ProjectileDamageOnHit>();
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        float safeBulletSpeed = Mathf.Max(0.01f, bulletSpeed);
        if (rb != null)
        {
            rb.velocity = transform.right * safeBulletSpeed;
        }

        float actualDistance = Mathf.Max(0.01f, attackRange * rangeToDistanceRatio);
        float lifeTime = actualDistance / safeBulletSpeed;
        projectileDamage.Configure(GetAttackDamage(), lifeTime, ReleaseBullet, this);
    }

    private void ReleaseBullet(GameObject bullet)
    {
        if (bullet == null)
        {
            return;
        }

        if (!activeBullets.Remove(bullet))
        {
            return;
        }

        if (bulletPool != null)
        {
            bulletPool.Release(bullet);
        }
        else
        {
            bullet.SetActive(false);
        }
    }

    private void PrepareBulletOnGet(GameObject bullet)
    {
        if (bullet != null)
        {
            bullet.transform.SetParent(null, true);
            bullet.SetActive(true);
        }
    }

    private void PrepareBulletOnRelease(GameObject bullet)
    {
        if (bullet == null)
        {
            return;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (poolRoot != null)
        {
            bullet.transform.SetParent(poolRoot, false);
        }

        bullet.SetActive(false);
    }

    private void ReleaseAllActiveBullets()
    {
        if (activeBullets.Count == 0)
        {
            return;
        }

        var bullets = new List<GameObject>(activeBullets);
        foreach (GameObject bullet in bullets)
        {
            ReleaseBullet(bullet);
        }

        activeBullets.Clear();
    }
}
