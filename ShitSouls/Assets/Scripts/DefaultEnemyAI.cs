using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class DefaultEnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public LayerMask playerLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 10f;
    public float attackCooldown = 2f;

    [Header("Attack Settings")]
    public float attackDamage = 20f;
    public float attackDuration = 0.5f;

    private NavMeshAgent agent;
    private CharacterController controller;
    private Transform player;
    private HealthManager playerHealth;
    private float lastAttackTime;
    private bool isAttacking;

    private enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    private EnemyState currentState = EnemyState.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        
        // Configure NavMeshAgent
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange * 0.8f; 
        agent.updateRotation = false; 
    }

    private void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<HealthManager>();
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // State machine
        switch (currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = EnemyState.Chase;
                }
                break;

            case EnemyState.Chase:
                if (distanceToPlayer > detectionRadius)
                {
                    currentState = EnemyState.Idle;
                    agent.isStopped = true;
                }
                else if (distanceToPlayer <= attackRange && !isAttacking)
                {
                    currentState = EnemyState.Attack;
                }
                else
                {
                    ChasePlayer();
                }
                break;

            case EnemyState.Attack:
                if (distanceToPlayer > attackRange)
                {
                    currentState = EnemyState.Chase;
                }
                else if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
                break;
        }

        // Always face the player when in chase or attack state
        if (currentState != EnemyState.Idle)
        {
            FacePlayer();
        }
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep rotation only on Y axis
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Stop movement during attack
        agent.isStopped = true;

        // Deal damage to player
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        // Resume movement after attack
        Invoke(nameof(EndAttack), attackDuration);
    }

    private void EndAttack()
    {
        isAttacking = false;
        agent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
