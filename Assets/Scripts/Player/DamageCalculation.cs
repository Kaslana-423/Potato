using UnityEngine;

public interface IDamageCalculator
{
    float Calculate(DamageContext context);
}

public readonly struct DamageContext
{
    public DamageContext(
        float flatDamage,
        float bonusPercent,
        PlayerStats stats,
        WeaponBase weapon)
    {
        FlatDamage = flatDamage;
        BonusPercent = bonusPercent;
        Stats = stats;
        Weapon = weapon;
    }

    public float FlatDamage { get; }
    public float BonusPercent { get; }
    public PlayerStats Stats { get; }
    public WeaponBase Weapon { get; }

    public DamageContext AddDamagePercent(float amount)
    {
        return new DamageContext(
            FlatDamage,
            BonusPercent + amount,
            Stats,
            Weapon);
    }
}

public sealed class BaseDamageCalculator : IDamageCalculator
{
    public float Calculate(DamageContext context)
    {
        float multiplier = Mathf.Max(0f, 1f + context.BonusPercent / 100f);
        return Mathf.Max(0f, context.FlatDamage) * multiplier;
    }
}

public abstract class DamageCalculatorDecorator : IDamageCalculator
{
    protected DamageCalculatorDecorator(IDamageCalculator inner)
    {
        Inner = inner;
    }

    protected IDamageCalculator Inner { get; }

    public abstract float Calculate(DamageContext context);
}

public sealed class MoveSpeedToDamageDecorator : DamageCalculatorDecorator
{
    private readonly PlayerStats stats;
    private readonly float damagePercentPerSpeedPoint;
    private readonly bool positiveSpeedOnly;

    public MoveSpeedToDamageDecorator(
        IDamageCalculator inner,
        PlayerStats stats,
        float damagePercentPerSpeedPoint,
        bool positiveSpeedOnly)
        : base(inner)
    {
        this.stats = stats;
        this.damagePercentPerSpeedPoint = Mathf.Max(0f, damagePercentPerSpeedPoint);
        this.positiveSpeedOnly = positiveSpeedOnly;
    }

    public override float Calculate(DamageContext context)
    {
        PlayerStats activeStats = stats != null ? stats : context.Stats;
        float speed = activeStats != null ? activeStats.Speed : 0f;
        if (positiveSpeedOnly)
        {
            speed = Mathf.Max(0f, speed);
        }

        float extraDamagePercent = speed * damagePercentPerSpeedPoint;
        return Inner.Calculate(context.AddDamagePercent(extraDamagePercent));
    }
}
