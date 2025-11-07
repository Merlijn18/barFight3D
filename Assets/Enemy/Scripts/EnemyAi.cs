using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyAI : MonoBehaviour
{
    [Header("Player Targeting")]
    public Transform player;
    public float chaseRange = 100f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("AI Components")]
    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime;

    // Callback voor WaveManager
    public Action onDeath;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Zet stoppingDistance op attackRange
        agent.stoppingDistance = attackRange;
        lastAttackTime = -attackCooldown; // zodat hij meteen kan aanvallen
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                // Stoppen en aanvallen
                agent.isStopped = true;
                animator.SetBool("isWalking", false);

                AttackPlayer();
                lastAttackTime = Time.time;
            }
            else
            {
                // Bewegen naar speler
                agent.isStopped = false;
                agent.SetDestination(player.position);

                // Smooth rotatie richting speler
                agent.updateRotation = false;
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0;

                if (direction.magnitude > 0.1f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }

                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy valt aan!");
        animator.SetTrigger("attack");
        // Hier kun je speler damage laten ontvangen, bijvoorbeeld:
        // player.GetComponent<PlayerHealth>().TakeDamage(10);
    }

    // 📉 Damage-functie voor als speler aanvalt
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("hit"); // optioneel, als je een hit-animatie hebt
        }
    }

    // 💀 Doodgaan van de vijand
    void Die()
    {
        Debug.Log("Enemy dood: " + gameObject.name);
        animator.SetTrigger("die");
        agent.isStopped = true;

        // WaveManager op de hoogte brengen
        onDeath?.Invoke();

        // Verwijder na korte tijd (zodat animatie kan afspelen)
        Destroy(gameObject, 2f);
    }
}
