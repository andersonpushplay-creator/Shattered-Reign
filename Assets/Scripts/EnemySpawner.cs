using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;

    public float spawnOffset = 6f;
    public float entrySpeed = 3f;
    public float delayBetweenEnemies = 1.2f;
    public float maxEntryTime = 3f;

    public IEnumerator SpawnWave()
    {
        Debug.Log("Spawner: começando wave.");

        StartCoroutine(SpawnEnemyRoutine(spawnPoint1));
        yield return new WaitForSeconds(delayBetweenEnemies);

        StartCoroutine(SpawnEnemyRoutine(spawnPoint2));
        yield return new WaitForSeconds(delayBetweenEnemies);

        StartCoroutine(SpawnEnemyRoutine(spawnPoint3));

        Debug.Log("Spawner: todos os inimigos foram chamados.");
    }

    IEnumerator SpawnEnemyRoutine(Transform point)
    {
        if (point == null || enemyPrefab == null)
        {
            Debug.LogWarning("Spawner: point ou enemyPrefab está null.");
            yield break;
        }

        Vector3 spawnPos = point.position;

        // define de qual lado nasce cada spawn point
        if (point == spawnPoint1)
        {
            spawnPos.x -= spawnOffset; // esquerda
        }
        else if (point == spawnPoint2)
        {
            spawnPos.x += spawnOffset; // direita
        }
        else if (point == spawnPoint3)
        {
            spawnPos.x += spawnOffset; // direita
        }

        Debug.Log("Spawner: criando inimigo em " + spawnPos);

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        yield return StartCoroutine(EnterArena(enemy, point.position));
    }

    IEnumerator EnterArena(GameObject enemy, Vector3 targetPos)
    {
        if (enemy == null) yield break;

        EnemyChase chase = enemy.GetComponent<EnemyChase>();
        EnemyAttack attack = enemy.GetComponent<EnemyAttack>();
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();

        if (attack != null)
            attack.ResetForSpawn();

        if (chase != null) chase.enabled = false;
        if (attack != null) attack.enabled = false;

        float timer = 0f;

        while (enemy != null &&
               Vector2.Distance(enemy.transform.position, targetPos) > 0.2f &&
               timer < maxEntryTime)
        {
            Vector2 dir = (targetPos - enemy.transform.position).normalized;

            if (rb != null)
                rb.linearVelocity = dir * entrySpeed;

            timer += Time.deltaTime;
            yield return null;
        }

        if (enemy == null) yield break;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        enemy.transform.position = targetPos;

        if (chase != null) chase.enabled = true;
        if (attack != null) attack.enabled = true;
        if (attack != null)
        {
            attack.ResetForSpawn();
        }

        Debug.Log("Spawner: inimigo entrou na arena.");
    }
}