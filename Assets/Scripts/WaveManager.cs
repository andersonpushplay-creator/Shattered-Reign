using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner spawner;
    public ArenaLock arenaLock;

    public int totalWaves = 2;

    bool waveStarted = false;

    public void StartWave()
    {
        if (waveStarted) return;

        waveStarted = true;
        Debug.Log("WaveManager: StartWave chamado.");
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        for (int wave = 1; wave <= totalWaves; wave++)
        {
            Debug.Log("Wave " + wave);

            if (EnemyAttackManager.I != null)
                EnemyAttackManager.I.ResetAll();

            if (spawner != null)
                yield return StartCoroutine(spawner.SpawnWave());

            Debug.Log("WaveManager: todos os inimigos da wave entraram.");

            if (arenaLock != null)
                arenaLock.LockArena();

            yield return new WaitUntil(() => FindFirstObjectByType<EnemyHealth>() == null);

            if (arenaLock != null)
                arenaLock.UnlockArena();

            yield return new WaitForSeconds(2f);
        }

        Debug.Log("Todas as waves concluídas.");
    }
}