using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;

        // 1. Tell the Animator to play the death animation
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // 2. Disable AI and Colliders so the "corpse" doesn't block the player
        if (GetComponent<UnityEngine.AI.NavMeshAgent>()) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        if (GetComponent<AlienAI>()) GetComponent<AlienAI>().enabled = false;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;

        // 3. Destroy the object after 3 seconds (letting the animation finish)
        Destroy(gameObject, 3f);
    }
}