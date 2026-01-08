using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    [Header("Audio")]
    public AudioClip damageSound;       // "ouch" geluid
    [Range(0f, 1f)] public float damageVolume = 1f; // volume van damage geluid

    private AudioSource audioSource;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        // Voeg AudioSource toe als die er nog niet is
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D geluid
        audioSource.volume = damageVolume;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        Debug.Log($"PlayerHealth initialized. HP: {currentHealth}/{maxHealth}");
    }

    // DAMAGE METHOD
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Player already dead, ignoring damage");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        // Speel damage geluid
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound, damageVolume);
        }

        Debug.Log($"Player HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // HEAL METHOD
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return; // speler dood, niet helen

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        Debug.Log($"Player healed by {amount}. HP: {currentHealth}/{maxHealth}");
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("PLAYER IS DEAD");
        Time.timeScale = 0f; // stop de tijd
    }
}
