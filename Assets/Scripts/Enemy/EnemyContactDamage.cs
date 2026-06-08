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

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player == null)
            {
                return;
            }

            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = player.gameObject.AddComponent<PlayerHealth>();
            }
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
