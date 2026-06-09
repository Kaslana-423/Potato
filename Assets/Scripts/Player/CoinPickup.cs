using UnityEngine;

public sealed class CoinPickup : MonoBehaviour
{
    [Header("Value")]
    [SerializeField, Min(1)] private int value = 1;

    [Header("Magnet")]
    [SerializeField, Min(0f)] private float magnetRadius = 3.5f;
    [SerializeField, Min(0f)] private float collectDistance = 0.25f;
    [SerializeField, Min(0f)] private float flySpeed = 7.5f;
    [SerializeField, Min(0f)] private float flyAcceleration = 24f;

    private Transform target;
    private float currentSpeed;
    private bool collecting;

    public int Value => value;

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
        if (!collecting && distance > magnetRadius)
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
        PlayerWallet.GetOrCreate().AddCoins(value);
        Destroy(gameObject);
    }
}
