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
    public Action<GameObject> onDeath;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!agent || !animator)
        {
            Debug.LogError("Enemy mist NavMeshAgent of Animator: " + gameObject.name);
            enabled = false;
            return;
        }

        agent.stoppingDistance = attackRange;
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (!agent.enabled || !player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                agent.isStopped = true;
                animator.SetBool("isWalking", false);

                AttackPlayer();
                lastAttackTime = Time.time;
            }
            else
            {
                agent.isStopped = false;

                if (agent.isOnNavMesh)
                    agent.SetDestination(player.position);

                RotateTowardsPlayer();
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void AttackPlayer()
    {
        animator.SetTrigger("attack");
        // TODO: speler damage geven
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
        else
            animator.SetTrigger("hit");
    }

    private void Die()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        animator.SetTrigger("die");
        animator.SetBool("isWalking", false);

        // WaveManager informeren
        onDeath?.Invoke(gameObject);
        onDeath = null; // voorkomt callback op destroyed object

        Destroy(gameObject, 2f);
    }
}
