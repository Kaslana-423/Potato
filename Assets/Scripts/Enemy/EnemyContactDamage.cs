using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private EnemyBase enemy;
    [SerializeField, Min(0f)] private float hitCooldownSeconds = 0.5f;

    private readonly Dictionary<PlayerHealth, float> nextHitTimes = new Dictionary<PlayerHealth, float>();

    private void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponentInParent<EnemyBase>();
        }
    }

    private void OnEnable()
    {
        nextHitTimes.Clear();
    }

    private void OnDisable()
    {
        nextHitTimes.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void TryDamagePlayer(Component other)
    {
        if (enemy == null || other == null)
        {
            return;
        }

        // Only a collider owned by the PlayerHealth object is a player hurtbox.
        // Weapon colliders are player children and must not forward contact damage to the player.
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            return;
        }

        if (nextHitTimes.TryGetValue(playerHealth, out float nextHitTime) && Time.time < nextHitTime)
        {
            return;
        }

        if (playerHealth.TakeDamage(enemy.Damage))
        {
            nextHitTimes[playerHealth] = Time.time + hitCooldownSeconds;
        }
    }
}
