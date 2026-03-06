using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AlienAI : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent agent;
    private Animator anim;
    public Transform player;

    [Header("State Settings")]
    public float sightRange = 15f;
    public float attackRange = 2f;
    public float chaseSpeed = 4f;

    [Header("Combat")]
    public float timeBetweenAttacks = 1.5f;
    public float attackDamage = 15f;
    private bool alreadyAttacked;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>(); // Find the Animator on this alien

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. Update Animation Speed
        // We pass the agent's current movement speed to the Animator
        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        // 2. State Logic
        if (distanceToPlayer <= attackRange) AttackPlayer();
        else if (distanceToPlayer <= sightRange) ChasePlayer();
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        agent.isStopped = true; // Stop moving to attack
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!alreadyAttacked)
        {
            // Trigger the Attack animation
            if (anim != null) anim.SetTrigger("Attack");

            // Damage the player
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(attackDamage);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); 
        }
    }

    private void ResetAttack() => alreadyAttacked = false;
}