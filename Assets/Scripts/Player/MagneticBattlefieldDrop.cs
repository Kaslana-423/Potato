using UnityEngine;

public abstract class MagneticBattlefieldDrop : BattlefieldDrop
{
    [Header("Magnet")]
    [SerializeField, Min(0f)] private float magnetRadius = 3.5f;
    [SerializeField, Min(0f)] private float collectDistance = 0.25f;
    [SerializeField, Min(0f)] private float flySpeed = 7.5f;
    [SerializeField, Min(0f)] private float flyAcceleration = 24f;

    private Transform target;
    private float currentSpeed;
    private bool collecting;

    protected virtual void OnEnable()
    {
        target = null;
        currentSpeed = 0f;
        collecting = false;
    }

    protected virtual void Update()
    {
        Transform player = ResolveTarget();
        if (player == null || (!collecting && !CanStartCollecting(player)))
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (!collecting && distance > GetEffectiveMagnetRadius())
        {
            return;
        }

        collecting = true;
        currentSpeed = Mathf.MoveTowards(currentSpeed, flySpeed, flyAcceleration * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);

        if (distance <= collectDistance && ApplyPickup(player))
        {
            Destroy(gameObject);
        }
    }

    protected virtual bool CanStartCollecting(Transform player)
    {
        return true;
    }

    protected abstract bool ApplyPickup(Transform player);

    private Transform ResolveTarget()
    {
        if (target != null)
        {
            return target;
        }

        if (PlayerStats.Instance != null)
        {
            target = PlayerStats.Instance.transform;
            return target;
        }

        PlayerHealth health = FindObjectOfType<PlayerHealth>();
        if (health != null)
        {
            target = health.transform;
            return target;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        target = player != null ? player.transform : null;
        return target;
    }

    private float GetEffectiveMagnetRadius()
    {
        int pickupRange = PlayerStats.Instance != null ? PlayerStats.Instance.PickupRange : 0;
        return magnetRadius * Mathf.Max(0f, 1f + pickupRange / 100f);
    }
}
