using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class EnemyAI : MonoBehaviour
{
    [Header("Player Targeting")]
    public Transform player;
    public float chaseRange = 100f;
    public float attackRange = 1.5f;
    public float attackCooldown = 3f;

    [Header("Attack Settings")]
    public int damage = 5;

    [Header("Health Settings")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Movement")]
    [Range(0.1f, 1f)]
    public float speedMultiplier = 0.5f; // base enemy speed
    public float slopeSpeedBoost = 2.5f; // multiplier voor snelheid omhoog

    [Header("Slope Handling")]
    public float groundCheckDistance = 2f;
    public float maxSlopeAngle = 50f;

    [Header("UI")]
    public Slider healthBar;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip damageSound;
    [Range(0f, 1f)] public float deathVolume = 1f;
    [Range(0f, 1f)] public float damageVolume = 0.6f;

    private AudioSource audioSource;
    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime = -999f;

    private RaycastHit slopeHit;

    public Action<GameObject> onDeath;
    public int CurrentHealth => currentHealth;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
            else
                Debug.LogError("Player object met tag 'Player' niet gevonden!");
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.stoppingDistance = attackRange;
            agent.speed *= speedMultiplier;
            agent.updateRotation = true;
            agent.updateUpAxis = true;
        }

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void Update()
    {
        if (player == null || currentHealth <= 0) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > chaseRange)
            StopMoving();
        else if (distance <= attackRange)
            TryAttack(distance);
        else
            ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;

            // Slope check
            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);

                // sneller omhoog lopen: hoe steiler de helling, hoe meer snelheid
                float speedFactor = 1f + (slopeAngle / maxSlopeAngle) * (slopeSpeedBoost - 1f);
                agent.speed = speedMultiplier * speedFactor;
            }
            else
            {
                agent.speed = speedMultiplier;
            }

            // NavMeshAgent doet zelf de slope correct
            agent.SetDestination(player.position);
        }

        RotateTowardsPlayer();
        if (animator != null)
            animator.SetBool("isWalking", true);
    }

    private void StopMoving()
    {
        if (agent != null && agent.enabled)
            agent.isStopped = true;

        if (animator != null)
            animator.SetBool("isWalking", false);
    }

    private void TryAttack(float distance)
    {
        StopMoving();
        RotateTowardsPlayer();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            if (animator != null)
                animator.SetTrigger("attack");

            if (distance <= attackRange + 0.4f)
                DealDamage();
        }
    }

    private void DealDamage()
    {
        if (player == null) return;

        PlayerHealth ph = player.GetComponentInChildren<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(damage);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound, damageVolume);

        if (currentHealth <= 0)
            Die();
        else if (animator != null)
            animator.SetTrigger("hit");
    }

    private void Die()
    {
        if (agent != null)
            agent.enabled = false;

        if (animator != null)
            animator.SetTrigger("die");

        onDeath?.Invoke(gameObject);

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound, deathVolume);
            Destroy(gameObject, deathSound.length);
        }
        else
        {
            Destroy(gameObject, 3f);
        }
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * 5f
            );
        }
    }

    // ===== SLOPE FUNCTIONS =====
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, groundCheckDistance))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > 0 && angle < maxSlopeAngle;
        }
        return false;
    }
}
