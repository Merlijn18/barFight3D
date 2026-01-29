using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 50f;
    public float fireRate = 0.5f;
    public int maxAmmo = 6;
    private int currentAmmo;

    [Header("Reload Settings")]
    public float reloadTimePerBullet = 0.3f; // tijd per kogel
    private bool isReloading = false;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound; // geluid voor 1 kogel reload
    [Range(0f, 1f)] public float volume = 1f;
    private AudioSource audioSource;

    [Header("Muzzle Flash")]
    public ParticleSystem muzzleFlashPS;
    [Range(0.8f, 1.5f)] public float minFlashScale = 0.8f;
    [Range(0.8f, 1.5f)] public float maxFlashScale = 1.2f;

    [Header("Light Flash")]
    public Light flashLight;
    public float flashDuration = 0.05f;

    [Header("UI")]
    public TextMeshProUGUI promptText; // Enkel centraal promptText object

    private float lastFireTime = 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;

        currentAmmo = maxAmmo;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = $"Ammo: {currentAmmo}/{maxAmmo}";
        }
    }

    private void Update()
    {
        if (isReloading) return;

        // 🔹 Shoot: linkermuisknop of rechter trigger
        if (Mouse.current.leftButton.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame))
        {
            Shoot();
        }

        // 🔹 Reload: R-toets of controller vierkantje / X
        if ((Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame))
        {
            if (currentAmmo < maxAmmo)
                StartCoroutine(Reload());
        }
    }

    public void Shoot()
    {
        if (Time.time - lastFireTime < fireRate) return;
        if (bulletPrefab == null || firePoint == null) return;

        if (currentAmmo <= 0)
        {
            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "Press R to reload!";
            }
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = firePoint.forward * bulletSpeed;

        Destroy(bullet, 1f);

        if (shootSound != null)
            audioSource.PlayOneShot(shootSound, volume);

        if (muzzleFlashPS != null)
        {
            muzzleFlashPS.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            float scale = Random.Range(minFlashScale, maxFlashScale);
            muzzleFlashPS.transform.localScale = Vector3.one * scale;
            muzzleFlashPS.Play();
        }

        if (flashLight != null)
            StartCoroutine(FlashLight());

        lastFireTime = Time.time;
        currentAmmo--;
        UpdateUI();
    }

    private IEnumerator FlashLight()
    {
        flashLight.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        flashLight.enabled = false;
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        // 🔹 Verberg promptText tijdens reload
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        int bulletsToReload = maxAmmo - currentAmmo;

        for (int i = 0; i < bulletsToReload; i++)
        {
            // 🔹 Speel reload geluid
            if (reloadSound != null)
                audioSource.PlayOneShot(reloadSound, volume);

            yield return new WaitForSeconds(reloadTimePerBullet);

            currentAmmo++;

            // Update promptText tijdens reload
            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = $"Reloading... {currentAmmo}/{maxAmmo}";
            }
        }

        // 🔹 Reload klaar
        isReloading = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(true);

            if (currentAmmo > 0)
                promptText.text = $"Ammo: {currentAmmo}/{maxAmmo}";
            else
                promptText.text = "Press R to reload!";
        }
    }
}
