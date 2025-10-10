using UnityEngine;
using System.Collections;

public class BeerSpawnManager : MonoBehaviour
{
    public static BeerSpawnManager Instance;   // Singleton
    public GameObject beerPrefab;              // Prefab van je bier
    public Transform[] spawnZones;             // Sleep hier je 6 zones in de Inspector

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Spawn in alle zones 1 bier
        foreach (Transform zone in spawnZones)
        {
            SpawnBeer(zone.position, zone.rotation);
        }
    }

    public void SpawnBeer(Vector3 pos, Quaternion rot)
    {
        Instantiate(beerPrefab, pos, rot);
    }

    public void RespawnBeer(Vector3 pos, Quaternion rot, float delay)
    {
        StartCoroutine(RespawnAfterDelay(pos, rot, delay));
    }

    private IEnumerator RespawnAfterDelay(Vector3 pos, Quaternion rot, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnBeer(pos, rot);
    }
}
