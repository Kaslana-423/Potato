using UnityEngine;

public enum WeaponRarity
{
    Common,    // 普通
    Uncommon,  // 罕见
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

    protected float currentCooldown = 0f;
    protected bool isAttacking = false; // 攻击状态标志，控制冷却的触发时机

    protected virtual void Update()
    {
        // 如果动作正在执行中（如正在挥刀），则暂停冷却计时
        if (isAttacking) return;

        // 简单的自动攻击逻辑：冷却完毕就攻击
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
        else
        {
            Attack();
        }
    }

    // 虚方法，留给近战和远程子类去具体实现
    protected virtual void Attack()
    {
        // 重置冷却时间
        // 因为加入了 isAttacking 状态，这段冷却时间实际上是在挥刀结束后才开始倒数的
        currentCooldown = attackCooldown;
    }
}