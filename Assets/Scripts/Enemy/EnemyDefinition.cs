using UnityEngine;

public enum EnemyCategory
{
    Regular,
    Elite,
    Boss,
    DlcRegular,
    DlcElite,
    DlcBoss
}

public sealed class EnemyDefinition
{
    public EnemyDefinition(
        string id,
        string displayName,
        EnemyCategory category,
        string behaviorDescription,
        float baseHealth,
        float healthPerWave,
        float minTableSpeed,
        float maxTableSpeed,
        float baseDamage,
        float damagePerWave,
        float knockbackResistance,
        int materialsDropped,
        float consumableDropChance,
        float lootCrateDropChance,
        int firstWave = 1,
        float armor = 0f,
        float armorPerWave = 0f)
    {
        Id = string.IsNullOrWhiteSpace(id) ? $"enemy.{displayName}".ToLowerInvariant() : id;
        DisplayName = displayName;
        Category = category;
        BehaviorDescription = behaviorDescription ?? string.Empty;
        BaseHealth = Mathf.Max(0f, baseHealth);
        HealthPerWave = Mathf.Max(0f, healthPerWave);
        MinTableSpeed = Mathf.Max(0f, minTableSpeed);
        MaxTableSpeed = Mathf.Max(MinTableSpeed, maxTableSpeed);
        BaseDamage = Mathf.Max(0f, baseDamage);
        DamagePerWave = Mathf.Max(0f, damagePerWave);
        KnockbackResistance = Mathf.Clamp01(knockbackResistance);
        MaterialsDropped = Mathf.Max(0, materialsDropped);
        ConsumableDropChance = Mathf.Clamp01(consumableDropChance);
        LootCrateDropChance = Mathf.Clamp01(lootCrateDropChance);
        FirstWave = Mathf.Max(1, firstWave);
        Armor = Mathf.Max(0f, armor);
        ArmorPerWave = Mathf.Max(0f, armorPerWave);
    }

    public string Id { get; }
    public string DisplayName { get; }
    public EnemyCategory Category { get; }
    public string BehaviorDescription { get; }
    public float BaseHealth { get; }
    public float HealthPerWave { get; }
    public float MinTableSpeed { get; }
    public float MaxTableSpeed { get; }
    public float BaseDamage { get; }
    public float DamagePerWave { get; }
    public float KnockbackResistance { get; }
    public int MaterialsDropped { get; }
    public float ConsumableDropChance { get; }
    public float LootCrateDropChance { get; }
    public int FirstWave { get; }
    public float Armor { get; }
    public float ArmorPerWave { get; }

    public bool IsDlc => Category == EnemyCategory.DlcRegular
        || Category == EnemyCategory.DlcElite
        || Category == EnemyCategory.DlcBoss;

    public bool IsElite => Category == EnemyCategory.Elite
        || Category == EnemyCategory.DlcElite;

    public bool IsBoss => Category == EnemyCategory.Boss
        || Category == EnemyCategory.DlcBoss;

    public float GetHealthForWave(int wave)
    {
        return Mathf.Max(1f, BaseHealth + HealthPerWave * GetWaveOffset(wave));
    }

    public float GetDamageForWave(int wave)
    {
        return BaseDamage + DamagePerWave * GetWaveOffset(wave);
    }

    public float GetArmorForWave(int wave)
    {
        return Armor + ArmorPerWave * GetWaveOffset(wave);
    }

    public float RollTableSpeed()
    {
        return MaxTableSpeed > MinTableSpeed
            ? Random.Range(MinTableSpeed, MaxTableSpeed)
            : MinTableSpeed;
    }

    private static int GetWaveOffset(int wave)
    {
        return Mathf.Max(0, wave - 1);
    }
}
