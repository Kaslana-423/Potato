using UnityEngine;

[CreateAssetMenu(
    fileName = "Move Speed To Damage Ability",
    menuName = "Potato/Character Abilities/Move Speed To Damage")]
public sealed class MoveSpeedToDamageAbilityDefinition : CharacterAbilityDefinition
{
    [SerializeField, Min(0f)] private float damagePercentPerSpeedPoint = 1f;
    [SerializeField] private bool positiveSpeedOnly = true;

    public float DamagePercentPerSpeedPoint => damagePercentPerSpeedPoint;
    public bool PositiveSpeedOnly => positiveSpeedOnly;

    public override string SelectionDescription
    {
        get
        {
            string speedCondition = positiveSpeedOnly ? "正移动速度" : "移动速度";
            return $"专属：每拥有 1% {speedCondition}，伤害提高 {damagePercentPerSpeedPoint:0.##}%";
        }
    }

    public override IDamageCalculator DecorateDamage(
        IDamageCalculator inner,
        PlayerStats stats)
    {
        return inner == null
            ? null
            : new MoveSpeedToDamageDecorator(
                inner,
                stats,
                damagePercentPerSpeedPoint,
                positiveSpeedOnly);
    }

    private void OnValidate()
    {
        damagePercentPerSpeedPoint = Mathf.Max(0f, damagePercentPerSpeedPoint);
    }
}
