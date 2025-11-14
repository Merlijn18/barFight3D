using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 25; // hoeveel schade de kogel doet
    private void OnTriggerEnter(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
