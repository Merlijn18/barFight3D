using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image foreground;
    private EnemyAI enemy;

    void Start()
    {
        enemy = GetComponentInParent<EnemyAI>();
    }

    void Update()
    {
        if (enemy == null || foreground == null) return;

        float healthPercent = (float)enemy.CurrentHealth / enemy.maxHealth;
        foreground.fillAmount = healthPercent;

        // Laat de balk altijd naar de camera kijken
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}