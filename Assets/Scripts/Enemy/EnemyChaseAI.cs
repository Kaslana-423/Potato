using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public sealed class EnemyChaseAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool findPlayerOnStart = true;
    [SerializeField] private float repathInterval = 0.5f;

    private EnemyBase enemy;
    private Rigidbody2D rb;
    private float repathTimer;

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

        Vector2 velocity = direction.normalized * enemy.MoveSpeed;
        if (rb != null)
        {
            rb.velocity = velocity;
        }
        else
        {
            transform.position += (Vector3)(velocity * Time.fixedDeltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
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
