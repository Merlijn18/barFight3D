using UnityEngine;
using System.Collections;

public class BeerSpawnManager : MonoBehaviour
{
    public static BeerSpawnManager Instance;

    [Header("Beer Settings")]
    public GameObject beerPrefab;        // Sleep hier je prefab in
    public Transform[] spawnZones;       // Spawnlocaties

    [Header("UI")]
    public UnityEngine.UI.Text promptText; // Sleep hier je Canvas Text in

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Spawn initieel bier op alle spawn zones
        foreach (Transform zone in spawnZones)
        {
            SpawnBeer(zone.position, zone.rotation);
        }
    }

    public void SpawnBeer(Vector3 pos, Quaternion rot)
    {
        if (beerPrefab == null)
        {
            Debug.LogError("Beer prefab is NOT assigned!");
            return;
        }

        Quaternion uprightRotation = Quaternion.Euler(-89.98f, 0f, 0f);

        // Instantiate de nieuwe beer
        GameObject newBeer = Instantiate(beerPrefab, pos, uprightRotation);

        // Koppel promptText
        BeerPickup beerPickup = newBeer.GetComponent<BeerPickup>();
        if (beerPickup != null)
        {
            beerPickup.promptText = promptText;
        }

        // Zorg dat collider exact gelijk is aan prefab
        Collider originalCol = beerPrefab.GetComponent<Collider>();
        Collider cloneCol = newBeer.GetComponent<Collider>();

        if (originalCol != null && cloneCol != null)
        {
            cloneCol.isTrigger = originalCol.isTrigger;

            // BoxCollider
            if (originalCol is BoxCollider originalBox && cloneCol is BoxCollider cloneBox)
            {
                cloneBox.center = originalBox.center;
                cloneBox.size = originalBox.size;
            }

            // SphereCollider
            if (originalCol is SphereCollider originalSphere && cloneCol is SphereCollider cloneSphere)
            {
                cloneSphere.center = originalSphere.center;
                cloneSphere.radius = originalSphere.radius;
            }

            // CapsuleCollider
            if (originalCol is CapsuleCollider originalCapsule && cloneCol is CapsuleCollider cloneCapsule)
            {
                cloneCapsule.center = originalCapsule.center;
                cloneCapsule.radius = originalCapsule.radius;
                cloneCapsule.height = originalCapsule.height;
                cloneCapsule.direction = originalCapsule.direction;
            }
        }
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
