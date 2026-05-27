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
        currentCooldown = attackCooldown;
    }

    protected virtual void UpdateWeaponRotation()
    {
        if (isAttacking) return;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        // 核心修复：在物理锁定角度的基础上，补偿贴图原图的偏差
        transform.rotation = Quaternion.Euler(0f, 0f, angle + visualRotationOffset);
    }
}