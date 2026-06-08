using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class WeaponDamageHitbox : MonoBehaviour
{
    [SerializeField] private WeaponBase weapon;
    [SerializeField] private bool requireActiveAttack = true;

    private readonly HashSet<EnemyBase> hitEnemies = new HashSet<EnemyBase>();
    private int trackedAttackSequence = -1;

    private void Awake()
    {
        if (weapon == null)
        {
            weapon = GetComponentInParent<WeaponBase>();
        }
    }

    private void OnEnable()
    {
        hitEnemies.Clear();
        trackedAttackSequence = -1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamageEnemy(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamageEnemy(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamageEnemy(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamageEnemy(collision.collider);
    }

    private void TryDamageEnemy(Component other)
    {
        if (weapon == null || other == null)
        {
            return;
        }

        if (requireActiveAttack && !weapon.IsAttacking)
        {
            return;
        }

        if (trackedAttackSequence != weapon.AttackSequence)
        {
            trackedAttackSequence = weapon.AttackSequence;
            hitEnemies.Clear();
        }

        EnemyBase enemy = other.GetComponentInParent<EnemyBase>();
        if (enemy == null || hitEnemies.Contains(enemy))
        {
            return;
        }

        hitEnemies.Add(enemy);
        enemy.TakeDamage(weapon.GetAttackDamage());
    }
}
