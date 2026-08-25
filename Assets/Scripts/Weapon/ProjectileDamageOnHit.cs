using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class ProjectileDamageOnHit : MonoBehaviour
{
    [SerializeField, Min(0f)] private float damage = 1f;
    [SerializeField] private bool releaseOnEnemyHit = true;

    private readonly HashSet<EnemyBase> hitEnemies = new HashSet<EnemyBase>();
    private Action<GameObject> releaseAction;
    private bool released;
    private int version;
    private float expiresAt;

    public int Version => version;

    public int Configure(float newDamage, float lifetime, Action<GameObject> newReleaseAction)
    {
        damage = Mathf.Max(0f, newDamage);
        releaseAction = newReleaseAction;
        released = false;
        hitEnemies.Clear();
        expiresAt = Time.time + Mathf.Max(0.01f, lifetime);
        version++;
        return version;
    }

    private void Update()
    {
        if (!released && Time.time >= expiresAt)
        {
            Release();
        }
    }

    private void OnDisable()
    {
        hitEnemies.Clear();
    }

    public void Expire(int expectedVersion)
    {
        if (expectedVersion == version)
        {
            Release();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamageEnemy(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamageEnemy(collision.collider);
    }

    private void TryDamageEnemy(Component other)
    {
        if (released || other == null)
        {
            return;
        }

        EnemyBase enemy = other.GetComponentInParent<EnemyBase>();
        if (enemy == null || hitEnemies.Contains(enemy))
        {
            return;
        }

        hitEnemies.Add(enemy);
        enemy.TakeDamage(damage);

        if (releaseOnEnemyHit)
        {
            Release();
        }
    }

    private void Release()
    {
        if (released)
        {
            return;
        }

        released = true;
        Action<GameObject> callback = releaseAction;
        releaseAction = null;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (callback != null)
        {
            callback(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
