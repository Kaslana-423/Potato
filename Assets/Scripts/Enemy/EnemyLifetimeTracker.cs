using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyLifetimeTracker
{
    private readonly List<EnemyBase> aliveEnemies = new List<EnemyBase>();
    private readonly Action<EnemyBase> releaseEnemy;
    private readonly Action<EnemyBase> enemyDied;

    public EnemyLifetimeTracker(Action<EnemyBase> releaseEnemy, Action<EnemyBase> enemyDied = null)
    {
        this.releaseEnemy = releaseEnemy;
        this.enemyDied = enemyDied;
    }

    public int AliveCount
    {
        get
        {
            TrimMissing();
            return aliveEnemies.Count;
        }
    }

    public void Track(EnemyBase enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.Died -= HandleEnemyDied;
        enemy.Died += HandleEnemyDied;
        aliveEnemies.Add(enemy);
    }

    public void TrimMissing()
    {
        for (int index = aliveEnemies.Count - 1; index >= 0; index--)
        {
            if (aliveEnemies[index] == null)
            {
                aliveEnemies.RemoveAt(index);
            }
        }
    }

    public void ReleaseAll()
    {
        for (int index = aliveEnemies.Count - 1; index >= 0; index--)
        {
            EnemyBase enemy = aliveEnemies[index];
            if (enemy == null)
            {
                continue;
            }

            enemy.Died -= HandleEnemyDied;
            releaseEnemy?.Invoke(enemy);
        }

        aliveEnemies.Clear();
    }

    private void HandleEnemyDied(EnemyBase enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.Died -= HandleEnemyDied;
        aliveEnemies.Remove(enemy);
        enemyDied?.Invoke(enemy);
        releaseEnemy?.Invoke(enemy);
    }
}
