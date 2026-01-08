using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ✅ NODIG voor scene switching

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    [Header("Audio")]
    public AudioClip damageSound;
    [Range(0f, 1f)] public float damageVolume = 1f;

    private AudioSource audioSource;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        Time.timeScale = 1f; // ✅ reset tijd (belangrijk bij reload)
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // DAMAGE
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (damageSound != null)
            audioSource.PlayOneShot(damageSound, damageVolume);

        if (currentHealth <= 0)
            Die();
    }

    // HEAL
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
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

        
        SceneManager.LoadScene("Menu"); // ✅ scene wissel
    }
}
