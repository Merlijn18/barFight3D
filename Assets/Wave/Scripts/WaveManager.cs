using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Instellingen")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform playerTarget;

    [Header("Timing en Aantallen")]
    [SerializeField] private int enemiesPerWave = 5;
    [SerializeField] private float delayBetweenSpawns = 1f;
    [SerializeField] private float delayBetweenWaves = 3f;

    private int currentWave = 0;
    private readonly List<GameObject> activeEnemies = new();
    private Coroutine waveRoutine;

    void Start()
    {
        // Player zoeken
        if (!playerTarget)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
                playerTarget = player.transform;
            else
            {
                Debug.LogError("Player met tag 'Player' niet gevonden.");
                return;
            }
        }

        // Validatie
        if (!enemyPrefab || !spawnPoint)
        {
            Debug.LogError("WaveManager mist enemyPrefab of spawnPoint.");
            return;
        }

        waveRoutine = StartCoroutine(WaveLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            currentWave++;
            Debug.Log($"Start wave {currentWave}");

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(delayBetweenSpawns);
            }

            yield return WaitForEnemiesToDie();

            Debug.Log($"Wave {currentWave} voltooid!");

            // Kleine difficulty scaling
            enemiesPerWave++;

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is niet gekoppeld!");
            return;
        }

        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI)
        {
            enemyAI.player = playerTarget;
            enemyAI.onDeath += OnEnemyDied;
        }

        activeEnemies.Add(enemy);
    }

    private void OnEnemyDied(GameObject enemy)
    {
        if (enemy != null)
            activeEnemies.Remove(enemy);
    }

    private IEnumerator WaitForEnemiesToDie()
    {
        while (activeEnemies.Any(e => e != null))
        {
            activeEnemies.RemoveAll(e => e == null);
            yield return null;
        }
    }
}
