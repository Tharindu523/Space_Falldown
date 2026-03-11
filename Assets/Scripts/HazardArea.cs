using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Deals damage to the player over time when they stay within a trigger volume.
/// Useful for fire, electricity, or toxic gas areas.
/// </summary>
public class HazardArea : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 10f;
    [Tooltip("The name of the hazard for debugging (e.g., 'Fire' or 'Electricity')")]
    public string hazardName = "Hazard";

    [Header("Visual/Audio Effects")]
    public GameObject damageEffectPrefab; // Optional: Sparks or heat haze
    public AudioClip sizzleSound;        // Optional: Loopable sound for the hazard

    private List<PlayerHealth> playersInRange = new List<PlayerHealth>();

    void Update()
    {
        // Apply damage to every player currently standing in the hazard
        if (playersInRange.Count > 0)
        {
            float damageThisFrame = damagePerSecond * Time.deltaTime;

            for (int i = 0; i < playersInRange.Count; i++)
            {
                if (playersInRange[i] != null)
                {
                    playersInRange[i].TakeDamage(damageThisFrame);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only track objects tagged as "Player" that have a PlayerHealth component
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && !playersInRange.Contains(health))
            {
                playersInRange.Add(health);
                Debug.Log("Player entered " + hazardName);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && playersInRange.Contains(health))
            {
                playersInRange.Remove(health);
                Debug.Log("Player exited " + hazardName);
            }
        }
    }
}