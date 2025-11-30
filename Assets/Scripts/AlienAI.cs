using UnityEngine;
using UnityEngine.AI; // REQUIRED for NavMeshAgent

/// <summary>
/// Controls the Alien's behavior, including patrol routes, 
/// detecting the player, chasing, and basic attacking.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))] // Ensure it has a Health component
public class AlienAI : MonoBehaviour
{
    // --- Public References ---
    [Header("References")]
    private NavMeshAgent agent;
    // Drag your Player's Transform object here (the main parent)
    public Transform player;

    // --- State & Movement ---
    [Header("State & Movement")]
    public float sightRange = 15f;    // Distance the alien can see the player
    public float attackRange = 2f;    // Distance at which the alien stops and attacks
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.5f;

    // --- Attacking ---
    [Header("Combat")]
    public float timeBetweenAttacks = 1.5f; // How often the alien can strike
    public float attackDamage = 15f;        // Damage dealt per attack
    private bool alreadyAttacked;

    // --- Internal State Tracking ---
    private bool playerInSightRange, playerInAttackRange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed; // Start slow

        // Safety check to ensure the player reference is set
        if (player == null)
        {
            Debug.LogError("AlienAI: Player Transform reference is missing. Drag the Player object here.");
            enabled = false;
        }
    }

    void Update()
    {
        // 1. Check for Player Proximity
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within sight and attack range
        playerInSightRange = distanceToPlayer <= sightRange;
        playerInAttackRange = distanceToPlayer <= attackRange;

        // 2. State Machine Logic
        if (playerInAttackRange) AttackPlayer();
        else if (playerInSightRange) ChasePlayer();
        // NOTE: Patrol logic is skipped for now to focus on attack/chase in a corridor.
        // We can add proper patrol routes in a later step!
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        Debug.Log("Alien: CHASING Player!");
    }

    void AttackPlayer()
    {
        // Stop movement while attacking
        agent.SetDestination(transform.position);

        // Face the player while attacking
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // --- ATTACK CODE ---
            Debug.Log("Alien: ATTACKING Player!");

            // 1. Find the Health component on the player
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            // 2. Reset attack state
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // This method is helpful for visualization in the Editor (optional)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}