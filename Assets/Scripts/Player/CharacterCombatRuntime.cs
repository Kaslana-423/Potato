using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerStats))]
public sealed class CharacterCombatRuntime : MonoBehaviour
{
    private readonly IDamageCalculator baseDamageCalculator = new BaseDamageCalculator();

    private PlayerStats playerStats;
    private IDamageCalculator damageCalculator;

    public CharacterDefinition Character { get; private set; }
    public IDamageCalculator DamageCalculator => damageCalculator ?? baseDamageCalculator;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        damageCalculator = baseDamageCalculator;
    }

    private void Start()
    {
        InitializeFromCharacterId(GameSessionState.CurrentCharacterId);
    }

    public void InitializeFromCharacterId(string characterId)
    {
        Initialize(CharacterCatalog.FindById(characterId));
    }

    public void Initialize(CharacterDefinition character)
    {
        Character = character;
        IDamageCalculator configuredCalculator = baseDamageCalculator;

        if (character != null)
        {
            var abilities = character.Abilities;
            for (int index = 0; index < abilities.Count; index++)
            {
                CharacterAbilityDefinition ability = abilities[index];
                if (ability == null)
                {
                    continue;
                }

                IDamageCalculator decorated = ability.DecorateDamage(
                    configuredCalculator,
                    playerStats);
                if (decorated != null)
                {
                    configuredCalculator = decorated;
                }
            }
        }

        damageCalculator = configuredCalculator;
    }

    public float CalculateDamage(float flatDamage, PlayerStats stats, WeaponBase weapon)
    {
        PlayerStats activeStats = stats != null ? stats : playerStats;
        float globalDamagePercent = activeStats != null ? activeStats.Damage : 0f;
        var context = new DamageContext(
            flatDamage,
            globalDamagePercent,
            activeStats,
            weapon);
        return DamageCalculator.Calculate(context);
    }
}
