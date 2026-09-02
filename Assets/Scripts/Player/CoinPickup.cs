using UnityEngine;

public sealed class CoinPickup : BattlefieldDrop
{
    [Header("Value")]
    [SerializeField, Min(1)] private int value = 1;
    [SerializeField, Min(1)] private int retainedMaterialUnits = 1;

    [Header("Magnet")]
    [SerializeField, Min(0f)] private float magnetRadius = 3.5f;
    [SerializeField, Min(0f)] private float collectDistance = 0.25f;
    [SerializeField, Min(0f)] private float flySpeed = 7.5f;
    [SerializeField, Min(0f)] private float flyAcceleration = 24f;

    private Transform target;
    private float currentSpeed;
    private bool collecting;

    public int Value => value;
    public int RetainedMaterialUnits => retainedMaterialUnits;
    public bool HasRetainedBonus => value > retainedMaterialUnits;

    private void OnEnable()
    {
        target = null;
        currentSpeed = 0f;
        collecting = false;
    }

    private void Update()
    {
        Transform player = ResolveTarget();
        if (player == null)
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

        if (distance <= collectDistance)
        {
            Collect();
        }
    }

    public void SetValue(int newValue)
    {
        value = Mathf.Max(1, newValue);
    }

    public void ConfigureMaterialValue(int collectedValue, int baseMaterialUnits)
    {
        value = Mathf.Max(1, collectedValue);
        retainedMaterialUnits = Mathf.Max(1, baseMaterialUnits);
    }

    private Transform ResolveTarget()
    {
        if (target != null)
        {
            return target;
        }

        PlayerWallet wallet = PlayerWallet.GetOrCreate();
        if (wallet != null)
        {
            target = wallet.transform;
            return target;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        target = player != null ? player.transform : null;
        return target;
    }

    private void Collect()
    {
        PlayerStats stats = PlayerStats.Instance;
        int collectedValue = value;
        if (stats != null
            && stats.DoubleMaterialChance > 0
            && Random.value < Mathf.Clamp01(stats.DoubleMaterialChance / 100f))
        {
            collectedValue *= 2;
        }

        PlayerWallet.GetOrCreate().AddCoins(collectedValue);
        PlayerExperience experience = PlayerExperience.GetOrCreate();
        experience?.AddMaterialExperience(collectedValue);

        if (stats != null
            && stats.MaterialsHealing > 0
            && Random.value < Mathf.Clamp01(stats.MaterialsHealing / 100f))
        {
            PlayerHealth health = stats.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Heal(1);
            }
        }

        Destroy(gameObject);
    }

    private float GetEffectiveMagnetRadius()
    {
        int pickupRange = PlayerStats.Instance != null ? PlayerStats.Instance.PickupRange : 0;
        return magnetRadius * Mathf.Max(0f, 1f + pickupRange / 100f);
    }
}
