using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public sealed class EnemySpawnPool : MonoBehaviour
{
    [SerializeField, Min(0)] private int defaultCapacity = 32;
    [SerializeField, Min(1)] private int maxPoolSizePerPrefab = 256;
    [SerializeField, Min(0)] private int prewarmCountPerPrefab = 8;
    [SerializeField] private bool collectionChecks = true;
    [SerializeField] private Transform poolRoot;

    [Header("Coin Drops")]
    [SerializeField] private CoinPickup coinPrefab;
    [SerializeField, Min(0)] private int defaultMinCoinsDropped = 1;
    [SerializeField, Min(0)] private int defaultMaxCoinsDropped = 3;
    [SerializeField, Min(0f)] private float defaultCoinScatterRadius = 0.45f;

    [Header("Consumable And Crate Drops")]
    [SerializeField] private FruitPickup fruitPrefab;
    [SerializeField] private LootCratePickup lootCratePrefab;
    [SerializeField, Min(0f)] private float defaultLootScatterRadius = 0.45f;

    private readonly Dictionary<EnemyBase, ObjectPool<EnemyBase>> prefabPools = new Dictionary<EnemyBase, ObjectPool<EnemyBase>>();
    private readonly Dictionary<EnemyBase, ObjectPool<EnemyBase>> instancePools = new Dictionary<EnemyBase, ObjectPool<EnemyBase>>();
    private readonly HashSet<EnemyBase> activeEnemies = new HashSet<EnemyBase>();
    private ObjectPool<EnemyBase> fallbackPool;
    private static Sprite fallbackSprite;

    public int ActiveCount => activeEnemies.Count;
    public int InactiveCount
    {
        get
        {
            int count = fallbackPool != null ? fallbackPool.CountInactive : 0;
            foreach (ObjectPool<EnemyBase> pool in prefabPools.Values)
            {
                count += pool.CountInactive;
            }

            return count;
        }
    }

    private void OnValidate()
    {
        defaultCapacity = Mathf.Max(0, defaultCapacity);
        maxPoolSizePerPrefab = Mathf.Max(1, maxPoolSizePerPrefab);
        prewarmCountPerPrefab = Mathf.Clamp(prewarmCountPerPrefab, 0, maxPoolSizePerPrefab);
    }

    private void OnDestroy()
    {
        foreach (ObjectPool<EnemyBase> pool in prefabPools.Values)
        {
            pool.Clear();
        }

        fallbackPool?.Clear();
        prefabPools.Clear();
        instancePools.Clear();
        activeEnemies.Clear();
    }

    public EnemyBase Get(EnemyBase prefab, Vector3 position, Quaternion rotation)
    {
        ObjectPool<EnemyBase> pool = prefab != null ? GetPrefabPool(prefab) : GetFallbackPool();
        EnemyBase enemy = pool.Get();
        instancePools[enemy] = pool;
        Transform enemyTransform = enemy.transform;
        enemyTransform.SetParent(null, true);
        enemyTransform.SetPositionAndRotation(position, rotation);
        enemy.gameObject.SetActive(true);
        activeEnemies.Add(enemy);
        return enemy;
    }

    public void Prewarm(EnemyBase prefab)
    {
        if (prefab != null)
        {
            GetPrefabPool(prefab);
        }
        else
        {
            GetFallbackPool();
        }
    }

    public bool Release(EnemyBase enemy)
    {
        if (enemy == null || !instancePools.TryGetValue(enemy, out ObjectPool<EnemyBase> pool))
        {
            return false;
        }

        if (!activeEnemies.Remove(enemy))
        {
            return true;
        }

        pool.Release(enemy);
        return true;
    }

    public void ReleaseAll(IEnumerable<EnemyBase> enemies)
    {
        if (enemies == null)
        {
            return;
        }

        foreach (EnemyBase enemy in enemies)
        {
            Release(enemy);
        }
    }

    private ObjectPool<EnemyBase> GetPrefabPool(EnemyBase prefab)
    {
        if (prefabPools.TryGetValue(prefab, out ObjectPool<EnemyBase> pool))
        {
            return pool;
        }

        pool = new ObjectPool<EnemyBase>(
            () => CreateFromPrefab(prefab),
            OnGet,
            OnRelease,
            OnDestroyPooledEnemy,
            collectionChecks,
            defaultCapacity,
            maxPoolSizePerPrefab);

        prefabPools.Add(prefab, pool);
        Prewarm(pool);
        return pool;
    }

    private ObjectPool<EnemyBase> GetFallbackPool()
    {
        if (fallbackPool != null)
        {
            return fallbackPool;
        }

        fallbackPool = new ObjectPool<EnemyBase>(
            CreateFallbackEnemy,
            OnGet,
            OnRelease,
            OnDestroyPooledEnemy,
            collectionChecks,
            defaultCapacity,
            maxPoolSizePerPrefab);

        Prewarm(fallbackPool);
        return fallbackPool;
    }

    private EnemyBase CreateFromPrefab(EnemyBase prefab)
    {
        EnemyBase enemy = Instantiate(prefab);
        PrepareEnemyInstance(enemy);
        return enemy;
    }

    private EnemyBase CreateFallbackEnemy()
    {
        GameObject enemyObject = new GameObject("Enemy");

        SpriteRenderer spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetFallbackSprite();
        spriteRenderer.color = new Color(0.95f, 0.28f, 0.2f, 1f);
        enemyObject.transform.localScale = Vector3.one * 0.6f;

        Rigidbody2D rb = enemyObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        CircleCollider2D collider = enemyObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        EnemyBase enemy = enemyObject.AddComponent<EnemyBase>();
        PrepareEnemyInstance(enemy);
        return enemy;
    }

    private void PrepareEnemyInstance(EnemyBase enemy)
    {
        if (enemy == null)
        {
            return;
        }

        if (enemy.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = enemy.gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        if (enemy.GetComponent<Collider2D>() == null)
        {
            enemy.gameObject.AddComponent<CircleCollider2D>();
        }

        if (enemy.GetComponent<EnemyChaseAI>() == null)
        {
            enemy.gameObject.AddComponent<EnemyChaseAI>();
        }

        if (enemy.GetComponent<EnemyContactDamage>() == null)
        {
            enemy.gameObject.AddComponent<EnemyContactDamage>();
        }

        EnemyCoinDropper coinDropper = enemy.GetComponent<EnemyCoinDropper>();
        bool addedCoinDropper = false;
        if (coinDropper == null)
        {
            coinDropper = enemy.gameObject.AddComponent<EnemyCoinDropper>();
            addedCoinDropper = true;
        }

        coinDropper.ConfigureDefaults(
            coinPrefab,
            defaultMinCoinsDropped,
            defaultMaxCoinsDropped,
            defaultCoinScatterRadius,
            addedCoinDropper);

        EnemyLootDropper lootDropper = enemy.GetComponent<EnemyLootDropper>();
        bool addedLootDropper = false;
        if (lootDropper == null)
        {
            lootDropper = enemy.gameObject.AddComponent<EnemyLootDropper>();
            addedLootDropper = true;
        }

        lootDropper.ConfigureDefaults(
            fruitPrefab,
            lootCratePrefab,
            defaultLootScatterRadius,
            addedLootDropper);

        enemy.gameObject.SetActive(false);
    }

    private void OnGet(EnemyBase enemy)
    {
        ResetPhysics(enemy);
    }

    private void OnRelease(EnemyBase enemy)
    {
        ResetPhysics(enemy);
        enemy.PrepareForPool();
        Transform enemyTransform = enemy.transform;
        if (poolRoot != null)
        {
            enemyTransform.SetParent(poolRoot, false);
        }

        enemy.gameObject.SetActive(false);
    }

    private void Prewarm(ObjectPool<EnemyBase> pool)
    {
        int count = Mathf.Min(prewarmCountPerPrefab, maxPoolSizePerPrefab);
        if (count <= 0)
        {
            return;
        }

        var enemies = new List<EnemyBase>(count);
        for (int index = 0; index < count; index++)
        {
            enemies.Add(pool.Get());
        }

        foreach (EnemyBase enemy in enemies)
        {
            pool.Release(enemy);
        }
    }

    private void OnDestroyPooledEnemy(EnemyBase enemy)
    {
        if (enemy != null)
        {
            instancePools.Remove(enemy);
            activeEnemies.Remove(enemy);
            Destroy(enemy.gameObject);
        }
    }

    private static void ResetPhysics(EnemyBase enemy)
    {
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private static Sprite GetFallbackSprite()
    {
        if (fallbackSprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            fallbackSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                texture.width);
        }

        return fallbackSprite;
    }
}
