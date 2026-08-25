using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public sealed class EnemyChaseAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool findPlayerOnStart = true;
    [SerializeField] private float repathInterval = 0.5f;
    [SerializeField, Min(0f)] private float knockbackUnitsPerPoint = 0.12f;
    [SerializeField, Min(0f)] private float knockbackDamping = 10f;

    private EnemyBase enemy;
    private Rigidbody2D rb;
    private float repathTimer;
    private Vector2 knockbackVelocity;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (findPlayerOnStart && target == null)
        {
            FindPlayerTarget();
        }
    }

    private void Update()
    {
        if (target != null)
        {
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            FindPlayerTarget();
        }
    }

    private void FixedUpdate()
    {
        if (target == null || enemy == null)
        {
            StopMoving();
            return;
        }

        Vector2 direction = target.position - transform.position;
        if (direction.sqrMagnitude <= 0.0001f)
        {
            StopMoving();
            return;
        }

        float playerEnemySpeed = PlayerStats.Instance != null ? PlayerStats.Instance.EnemySpeed : 0f;
        float speedMultiplier = Mathf.Max(0f, 1f + playerEnemySpeed / 100f);
        Vector2 velocity = direction.normalized * enemy.MoveSpeed * speedMultiplier + knockbackVelocity;
        if (rb != null)
        {
            rb.velocity = velocity;
        }
        else
        {
            transform.position += (Vector3)(velocity * Time.fixedDeltaTime);
        }

        knockbackVelocity = Vector2.MoveTowards(
            knockbackVelocity,
            Vector2.zero,
            knockbackDamping * Time.fixedDeltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ApplyKnockback(Vector2 direction, float knockbackPoints)
    {
        if (direction.sqrMagnitude <= 0.0001f || knockbackPoints <= 0f)
        {
            return;
        }

        knockbackVelocity += direction.normalized * knockbackPoints * knockbackUnitsPerPoint;
    }

    private void OnDisable()
    {
        StopMoving();
        target = null;
        repathTimer = 0f;
        knockbackVelocity = Vector2.zero;
    }

    private void StopMoving()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void FindPlayerTarget()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            target = player.transform;
            return;
        }

        GameObject taggedPlayer = GameObject.FindGameObjectWithTag("Player");
        if (taggedPlayer != null)
        {
            target = taggedPlayer.transform;
        }
    }
}
