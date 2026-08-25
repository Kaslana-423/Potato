using UnityEngine;

public sealed class EnemyCoinDropper : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] private CoinPickup coinPrefab;
    [SerializeField, Min(0)] private int minCoinsDropped = 1;
    [SerializeField, Min(0)] private int maxCoinsDropped = 3;
    [SerializeField, Min(0f)] private float scatterRadius = 0.45f;
    [SerializeField, Min(1)] private int coinValue = 1;

    private bool droppedThisLife;

    private void OnEnable()
    {
        droppedThisLife = false;
    }

    private void OnValidate()
    {
        minCoinsDropped = Mathf.Max(0, minCoinsDropped);
        maxCoinsDropped = Mathf.Max(minCoinsDropped, maxCoinsDropped);
        coinValue = Mathf.Max(1, coinValue);
    }

    public void ConfigureDefaults(
        CoinPickup defaultCoinPrefab,
        int defaultMinCoins,
        int defaultMaxCoins,
        float defaultScatterRadius,
        bool overwriteDropSettings)
    {
        if (coinPrefab == null)
        {
            coinPrefab = defaultCoinPrefab;
        }

        if (overwriteDropSettings)
        {
            minCoinsDropped = Mathf.Max(0, defaultMinCoins);
            maxCoinsDropped = Mathf.Max(minCoinsDropped, defaultMaxCoins);
            scatterRadius = Mathf.Max(0f, defaultScatterRadius);
        }
    }

    public void DropCoins()
    {
        if (droppedThisLife || coinPrefab == null)
        {
            return;
        }

        droppedThisLife = true;
        int amount = Random.Range(minCoinsDropped, maxCoinsDropped + 1);
        for (int index = 0; index < amount; index++)
        {
            Vector2 offset = scatterRadius > 0f ? Random.insideUnitCircle * scatterRadius : Vector2.zero;
            CoinPickup coin = Instantiate(coinPrefab, transform.position + (Vector3)offset, Quaternion.identity);
            PlayerWallet wallet = PlayerWallet.GetOrCreate();
            int retainedBonus = wallet != null ? wallet.ConsumeRetainedMaterialBonus(coinValue) : 0;
            coin.ConfigureMaterialValue(coinValue + retainedBonus, coinValue);
        }
    }
}
