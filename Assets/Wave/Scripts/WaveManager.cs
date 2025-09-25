using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;             // Prefab vijand
    public Transform spawnPoint;                // Waar vijanden spawnen (bij de deur)
    public DoubleDoorTrigger doorTrigger;       // Trigger die deuren bedient

    public int enemiesPerWave = 5;              // Aantal vijanden per wave
    public float timeBetweenWaves = 5f;         // Tijd tussen waves

    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        currentWave++;
        Debug.Log("Start wave " + currentWave);

        doorTrigger.SetEnemiesToPass(enemiesPerWave);  // Zet aantal vijanden dat moet passeren
        doorTrigger.OpenDoors();                        // Open de deuren

        yield return new WaitForSeconds(1.5f);          // Wacht tot deuren open zijn

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f);
        }

        // Wacht hier eventueel op vijanden dood als je dat wil

        yield return new WaitForSeconds(timeBetweenWaves);

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
    }
}
