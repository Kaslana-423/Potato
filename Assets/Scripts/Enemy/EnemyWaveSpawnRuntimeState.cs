public sealed class EnemyWaveSpawnRuntimeState
{
    public EnemyWaveSpawnRuntimeState(
        EnemyDefinition definition,
        EnemyWaveEnemySpawnRule rule,
        int waveOffset)
    {
        this.definition = definition;
        this.rule = rule;
        this.waveOffset = waveOffset;
        nextSpawnTime = 0f;
    }

    public readonly EnemyDefinition definition;
    public readonly EnemyWaveEnemySpawnRule rule;
    public float nextSpawnTime;
    public int waveOffset;
}
