using UnityEngine;

/// <summary>
/// A generic health component that can be attached to any object 
/// (Player, Enemy, Breakable) that needs to take damage and die.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Reduces current health by the specified amount.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to inflict.</param>
    public void TakeDamage(float damageAmount)
    {
        // Prevent damage if the object is already dead
        if (currentHealth <= 0) return;

        // Apply the damage
        currentHealth -= damageAmount;
        Debug.Log(transform.name + " took " + damageAmount + " damage. Remaining Health: " + currentHealth);

        // Check if the object has died
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Called when health drops to zero or below.
    /// </summary>
    void Die()
    {
        Debug.Log(transform.name + " has died!");

        // --- TODO: Add death logic here ---
        // For a simple target, we just destroy the object.
        Destroy(gameObject);

        // For the player, you would typically load a game over scene.
        // For an enemy, you might trigger an animation, drop loot, etc.
    }
}