using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    // Property om currentHealth van buitenaf te lezen
    public int CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        Debug.Log($"PlayerHealth initialized. HP: {currentHealth}/{maxHealth}");
    }

    // Wordt aangeroepen door Enemy
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Player already dead, ignoring damage");
            return;
        }

        Debug.Log($"TakeDamage called with {damage}");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        Debug.Log($"Player HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
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
        // Stop tijd of movement
        Time.timeScale = 0f;
    }
}