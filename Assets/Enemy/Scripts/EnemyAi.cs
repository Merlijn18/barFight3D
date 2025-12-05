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
    public int currentHealth;

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

        // Zorg ervoor dat de agent is geladen en op de NavMesh staat
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component ontbreekt op " + gameObject.name);
            return;
        }

        // Zet stoppingDistance op attackRange
        agent.stoppingDistance = attackRange;
        lastAttackTime = -attackCooldown; // zodat hij meteen kan aanvallen
    }

    void Update()
    {
        // Controleer of de agent actief is.
        if (agent == null || !agent.enabled) return;
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

                // Voorkom fout als de speler niet op de NavMesh is (hoewel de agent.SetDestination dit al opvangt)
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(player.position);
                }

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
            // Buiten bereik: stoppen
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy valt aan!");
        animator.SetTrigger("attack");
        // Hier kun je speler damage laten ontvangen
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
            animator.SetTrigger("hit"); // optioneel
        }
    }

    // 💀 Doodgaan van de vijand
    void Die()
    {
        Debug.Log("Enemy dood: " + gameObject.name);
        animator.SetTrigger("die");

        // FIX: Zorg ervoor dat de NavMeshAgent stopt en wordt uitgeschakeld om de "Stop" error te voorkomen.
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false; // CRUCIALE FIX
        }
        animator.SetBool("isWalking", false);

        // WaveManager op de hoogte brengen
        onDeath?.Invoke();

        // Verwijder na korte tijd (zodat animatie kan afspelen)
        Destroy(gameObject, 2f);
    }
}