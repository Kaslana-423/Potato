using UnityEngine;

public abstract class CharacterAbilityDefinition : ScriptableObject
{
    public abstract string SelectionDescription { get; }

    public abstract IDamageCalculator DecorateDamage(
        IDamageCalculator inner,
        PlayerStats stats);
}
