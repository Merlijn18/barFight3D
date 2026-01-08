using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;   // prefab van de kogel
    public Transform firePoint;       // plek waar de kogel vandaan komt
    public float bulletSpeed = 50f;   // snelheid van de kogel
    public float fireRate = 0.5f;     // tijd tussen schoten

    private float lastFireTime = 0f;

    [Header("Audio")]
    public AudioClip shootSound;      // geluidsbestand voor schot
    [Range(0f, 1f)] public float shootVolume = 0.5f; // volume, 0 = stil, 1 = max
    private AudioSource audioSource;  // audio source component

    private void Awake()
    {
        // Voeg AudioSource toe als deze nog niet bestaat
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.volume = shootVolume; // zet standaard volume van audioSource
    }

    public void Shoot()
    {
        if (Time.time - lastFireTime < fireRate) return;
        if (bulletPrefab == null || firePoint == null) return;

        // Spawn kogel met zelfde rotatie als firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Haal Rigidbody op
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }

        // Vernietig kogel na 1 seconde
        Destroy(bullet, 1f);

        // Speel het schotgeluid zachter
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }

        lastFireTime = Time.time;
    }
}
