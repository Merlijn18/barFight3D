using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 50f;
    public float fireRate = 0.5f;

    private float lastFireTime = 0f;

    [Header("Audio")]
    public AudioClip shootSound;
    [Range(0f, 1f)] public float shootVolume = 1f;
    private AudioSource audioSource;

    [Header("Muzzle Flash")]
    public ParticleSystem muzzleFlashPS;   // voeg je Particle System hier toe
    [Range(0.8f, 1.5f)] public float minFlashScale = 0.8f;
    [Range(0.8f, 1.5f)] public float maxFlashScale = 1.2f;

    [Header("Light Flash")]
    public Light flashLight;                // voeg je Point Light hier toe
    public float flashDuration = 0.05f;     // hoe lang het licht aan blijft

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume = shootVolume;
        audioSource.spatialBlend = 0f; // 2D geluid
    }

    public void Shoot()
    {
        if (Time.time - lastFireTime < fireRate) return;
        if (bulletPrefab == null || firePoint == null) return;

        // 🔹 Spawn kogel
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = firePoint.forward * bulletSpeed; // ✅ correct property

        Destroy(bullet, 1f);

        // 🔹 Speel geluid
        if (shootSound != null)
            audioSource.PlayOneShot(shootSound, shootVolume);

        // 🔹 Muzzle flash
        if (muzzleFlashPS != null)
        {
            // Random rotation
            muzzleFlashPS.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            // Random scale
            float scale = Random.Range(minFlashScale, maxFlashScale);
            muzzleFlashPS.transform.localScale = Vector3.one * scale;

            muzzleFlashPS.Play();
        }

        // 🔹 Lichtflits
        if (flashLight != null)
            StartCoroutine(FlashLight());

        lastFireTime = Time.time;
    }

    private IEnumerator FlashLight()
    {
        flashLight.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        flashLight.enabled = false;
    }
}
