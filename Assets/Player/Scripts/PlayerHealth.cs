using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    public int CurrentHealth => currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        Debug.Log($"PlayerHealth initialized. HP: {currentHealth}/{maxHealth}");
    }

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

        Debug.Log($"Player HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

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
