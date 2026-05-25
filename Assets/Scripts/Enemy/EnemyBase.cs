using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("敌人基础属性")]
    public float maxHealth = 100f;
    public float currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}