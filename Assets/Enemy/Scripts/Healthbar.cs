using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("UI Components")]
    public Image foreground; // Sleep hier de voorgrond van je healthbar in (fillAmount gaat van 1 naar 0)
    public Image background; // Optioneel: voor een achtergrond

    [Header("Enemy Settings")]
    public EnemyAI enemy; // Sleep hier de Enemy in waarvan je de health wilt laten zien

    void Update()
    {
        if (enemy == null || enemy.maxHealth <= 0) return;

        // Bereken health percentage: 1.0 is vol, 0.0 is leeg. Dit is de correcte logica.
        float healthPercent = (float)enemy.currentHealth / enemy.maxHealth;

        // Update de UI
        if (foreground != null)
        {
            // foreground.fillAmount wordt geüpdatet van 1.0 (vol) naar 0.0 (leeg)
            foreground.fillAmount = Mathf.Clamp01(healthPercent);
        }

        // Optioneel: Zorgt ervoor dat de healthbar naar de camera kijkt (voor World Space Canvassen)
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}