using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private string enemyId = "enemy.custom";
    [SerializeField] private string displayName = "Custom Enemy";
    [SerializeField] private EnemyCategory category = EnemyCategory.Regular;
    [SerializeField] private string behaviorDescription = string.Empty;

    [Header("Combat")]
    public float maxHealth = 100f;
    public float currentHealth;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float armor = 0f;
    [SerializeField] private float knockbackResistance = 0f;

    [Header("Movement")]
    [SerializeField] private float tableSpeed = 200f;
    [SerializeField] private float tableSpeedToUnityUnits = 0.01f;

    [Header("Drops")]
    [SerializeField] private int materialsDropped = 1;
    [SerializeField, Range(0f, 1f)] private float consumableDropChance = 0.01f;
    [SerializeField, Range(0f, 1f)] private float lootCrateDropChance = 0.01f;

    private bool initialized;

    public event Action<EnemyBase> Died;

    public string EnemyId => enemyId;
    public string DisplayName => displayName;
    public EnemyCategory Category => category;
    public string BehaviorDescription => behaviorDescription;
    public float Damage => damage;
    public float Armor => armor;
    public float KnockbackResistance => knockbackResistance;
    public float TableSpeed => tableSpeed;
    public float MoveSpeed => Mathf.Max(0f, tableSpeed * tableSpeedToUnityUnits);
    public int MaterialsDropped => materialsDropped;
    public float ConsumableDropChance => consumableDropChance;
    public float LootCrateDropChance => lootCrateDropChance;

    protected virtual void Start()
    {
        if (!initialized)
        {
            currentHealth = maxHealth;
        }
    }

    public virtual void Initialize(EnemyDefinition definition, int wave)
    {
        if (definition == null)
        {
            return;
        }

        enemyId = definition.Id;
        displayName = definition.DisplayName;
        category = definition.Category;
        behaviorDescription = definition.BehaviorDescription;
        maxHealth = definition.GetHealthForWave(wave);
        currentHealth = maxHealth;
        damage = definition.GetDamageForWave(wave);
        armor = definition.GetArmorForWave(wave);
        knockbackResistance = definition.KnockbackResistance;
        tableSpeed = definition.RollTableSpeed();
        materialsDropped = definition.MaterialsDropped;
        consumableDropChance = definition.ConsumableDropChance;
        lootCrateDropChance = definition.LootCrateDropChance;
        gameObject.name = $"Enemy - {displayName}";
        initialized = true;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Max(0f, damage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        EnemyCoinDropper coinDropper = GetComponent<EnemyCoinDropper>();
        if (coinDropper != null)
        {
            coinDropper.DropCoins();
        }

        Died?.Invoke(this);
        if (gameObject.activeSelf)
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
