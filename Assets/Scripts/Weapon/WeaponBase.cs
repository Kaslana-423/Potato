using UnityEngine;

public enum WeaponRarity { Common, Rare, Epic, Legendary }

public abstract class WeaponBase : MonoBehaviour
{
    [Header("基础属性")]
    public string weaponName;
    public Sprite weaponIcon;
    [TextArea] public string description;
    public float attackPower = 10f;
    public float attackRange = 5f;
    public WeaponRarity rarity = WeaponRarity.Common;

    [Header("攻击设置")]
    public float attackCooldown = 1f;

    [Header("视觉偏置")]
    [Tooltip("如果武器原图剑尖朝上(Y轴)，填 -90；如果朝右上角对角线，填 -45")]
    public float visualRotationOffset = 0f;

    protected PlayerTargeting targetingSystem;
    protected Vector3 aimDirection = Vector3.right;
    protected float currentCooldown = 0f;
    protected bool isAttacking = false;
    private int attackSequence;
    private ShopWeaponDefinition runtimeDefinition;
    private float runtimeBaseDamageBonus;
    private float nextLifeStealTime;

    public bool IsAttacking => isAttacking;
    public int AttackSequence => attackSequence;

    protected virtual void Awake()
    {
        targetingSystem = GetComponentInParent<PlayerTargeting>();
    }

    protected virtual void Update()
    {
        // 【核心修复】：让武器根据自己的位置重新计算向量，消除视差
        if (targetingSystem != null && targetingSystem.ClosestEnemy != null)
        {
            aimDirection = (targetingSystem.ClosestEnemy.position - transform.position).normalized;
        }

        UpdateWeaponRotation();

        // 状态机接管了攻击过程，这里只负责冷却倒计时
        if (isAttacking) return;

        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
        else
        {
            // 只有雷达锁定了目标才开火
            if (targetingSystem != null && targetingSystem.ClosestEnemy != null)
            {
                Attack();
            }
        }
    }

    protected virtual void Attack()
    {
        attackSequence++;
        currentCooldown = GetEffectiveAttackCooldown();
    }

    public virtual float GetAttackDamage()
    {
        PlayerStats stats = PlayerStats.Instance;
        float flatDamage = runtimeDefinition != null
            ? runtimeDefinition.CalculateDamage(stats) + runtimeBaseDamageBonus
            : attackPower;
        if (stats != null)
        {
            if (runtimeDefinition == null)
            {
                flatDamage += GetFlatDamageBonus(stats);
            }

            flatDamage *= 1f + stats.Damage / 100f;
        }

        float damage = Mathf.Ceil(Mathf.Max(0f, flatDamage));
        float critChance = (runtimeDefinition != null ? runtimeDefinition.CritChance : 0f)
            + (stats != null ? stats.CritChance : 0f);
        if (critChance > 0f && Random.value < Mathf.Clamp01(critChance / 100f))
        {
            float critMultiplier = runtimeDefinition != null
                ? Mathf.Max(1f, runtimeDefinition.CritMultiplier)
                : 1.5f;
            damage = Mathf.Ceil(damage * critMultiplier);
        }

        return damage;
    }

    public void ConfigureRuntimeDefinition(ShopWeaponDefinition definition, float baseDamageBonus = 0f)
    {
        runtimeDefinition = definition;
        runtimeBaseDamageBonus = baseDamageBonus;
    }

    public float ModifyDamageForTarget(float damage, EnemyBase enemy)
    {
        PlayerStats stats = PlayerStats.Instance;
        if (enemy != null
            && stats != null
            && (enemy.Category == EnemyCategory.Boss || enemy.Category == EnemyCategory.DlcBoss))
        {
            damage *= Mathf.Max(0f, 1f + stats.DamageAgainstBosses / 100f);
        }

        return Mathf.Ceil(Mathf.Max(0f, damage));
    }

    public float GetKnockback()
    {
        float knockback = runtimeDefinition != null ? runtimeDefinition.Knockback : 0f;
        if (PlayerStats.Instance != null)
        {
            knockback += PlayerStats.Instance.Knockback;
        }

        return Mathf.Max(0f, knockback);
    }

    public void HandleSuccessfulHit(float damage)
    {
        if (damage <= 0f || Time.time < nextLifeStealTime)
        {
            return;
        }

        PlayerStats stats = PlayerStats.Instance;
        float lifeStealChance = runtimeDefinition != null ? runtimeDefinition.LifeSteal : 0f;
        if (stats != null)
        {
            lifeStealChance += stats.LifeSteal;
        }

        if (lifeStealChance <= 0f || Random.value >= Mathf.Clamp01(lifeStealChance / 100f))
        {
            return;
        }

        PlayerHealth health = stats != null ? stats.GetComponent<PlayerHealth>() : null;
        if (health != null)
        {
            health.Heal(1);
            nextLifeStealTime = Time.time + 0.1f;
        }
    }

    public float GetEffectiveAttackCooldown()
    {
        float attackSpeed = PlayerStats.Instance != null ? PlayerStats.Instance.AttackSpeed : 0f;
        return Mathf.Max(0.01f, attackCooldown / Mathf.Max(0.1f, 1f + attackSpeed / 100f));
    }

    protected virtual float GetFlatDamageBonus(PlayerStats stats)
    {
        return stats != null ? stats.MeleeDamage : 0f;
    }

    protected void EnsureDamageHitboxesInChildren()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<WeaponDamageHitbox>() == null)
            {
                collider.gameObject.AddComponent<WeaponDamageHitbox>();
            }
        }
    }

    protected virtual void UpdateWeaponRotation()
    {
        if (isAttacking) return;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        // 核心修复：在物理锁定角度的基础上，补偿贴图原图的偏差
        transform.rotation = Quaternion.Euler(0f, 0f, angle + visualRotationOffset);
    }
}
