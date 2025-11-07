using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;   // prefab van de kogel
    public Transform firePoint;       // plek waar de kogel vandaan komt
    public float bulletSpeed = 50f;   // snelheid van de kogel
    public float fireRate = 0.5f;     // tijd tussen schoten

    private float lastFireTime = 0f;

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
            // Zorg dat kogel in de "voorwaartse" richting van firePoint gaat
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }

        // Vernietig kogel na 1 seconde
        Destroy(bullet, 1f);

        lastFireTime = Time.time;
    }
}
