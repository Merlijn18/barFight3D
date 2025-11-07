using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;              // Prefab van de vijand
    public Transform spawnPoint;                 // Waar vijanden spawnen (bij de deur)
    public DoubleDoorTrigger doorTrigger;        // Trigger die deuren bedient

    public int enemiesPerWave = 5;               // Aantal vijanden per wave
    public float delayBetweenSpawns = 1f;        // Tijd tussen spawns van individuele vijanden
    public float delayBetweenWaves = 3f;         // Optioneel: korte pauze tussen waves

    private int currentWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        currentWave++;
        Debug.Log("Start wave " + currentWave);

        doorTrigger.SetEnemiesToPass(enemiesPerWave);
        doorTrigger.OpenDoors();

        yield return new WaitForSeconds(1.5f); // Wacht tot deuren open zijn

        // Spawn alle vijanden
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        // Wacht tot alle vijanden dood zijn
        yield return StartCoroutine(WaitForEnemiesToDie());

        // Optioneel: korte pauze tussen waves
        yield return new WaitForSeconds(delayBetweenWaves);

        // Start volgende wave
        StartCoroutine(StartNextWave());
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            Debug.LogWarning("EnemyAI script niet gevonden op vijand prefab!");
        }

        activeEnemies.Add(enemy);
        enemy.GetComponent<EnemyAI>().onDeath += () => OnEnemyDied(enemy);

    }

    void OnEnemyDied(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    IEnumerator WaitForEnemiesToDie()
    {
        // Wacht tot alle vijanden uit de lijst verdwenen zijn
        while (activeEnemies.Count > 0)
        {
            yield return null;
        }

        Debug.Log("Wave " + currentWave + " is voorbij!");
    }
}
