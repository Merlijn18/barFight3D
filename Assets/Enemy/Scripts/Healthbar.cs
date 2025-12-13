using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("UI Components")]
    public Image foreground; // Sleep hier de voorgrond van je healthbar in (fillAmount gaat van 1 naar 0)
    public Image background; // Optioneel: voor een achtergrond

    [Header("Enemy Settings")]
    public EnemyAI enemy; // Wordt automatisch opgehaald als het niet handmatig is gekoppeld

    void Start()
    {
        // Als enemy nog niet gekoppeld is, zoek het component in parent prefab
        if (enemy == null)
        {
            enemy = GetComponentInParent<EnemyAI>();
            if (enemy == null)
            {
                Debug.LogError("Healthbar kan geen EnemyAI vinden in parent prefab!");
            }
        }
    }

    void Update()
    {
        if (enemy == null || enemy.maxHealth <= 0) return;

        // Bereken health percentage
        float healthPercent = (float)enemy.currentHealth / enemy.maxHealth;

        // Update de UI
        if (foreground != null)
        {
            foreground.fillAmount = Mathf.Clamp01(healthPercent);
        }

        // Laat de healthbar naar de camera kijken (voor World Space Canvas)
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
