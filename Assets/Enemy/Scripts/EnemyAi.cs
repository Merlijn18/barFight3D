using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Zet stoppingDistance op attackRange, zodat agent automatisch stopt vlakbij target
        agent.stoppingDistance = attackRange;

        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown; // zodat enemy meteen kan aanvallen als in range
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                // Stop bewegen om te kunnen aanvallen
                agent.isStopped = true;
                animator.SetBool("isWalking", false);

                AttackPlayer();
                lastAttackTime = Time.time;
            }
            else
            {
                // Beweeg naar de speler, stopt automatisch op stoppingDistance
                agent.isStopped = false;
                agent.SetDestination(player.position);

                // Zelf rotatie regelen voor smooth draaien
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
            // Buiten chase range, stop bewegen en animatie
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy valt aan!");
        animator.SetTrigger("attack");
    }
}
