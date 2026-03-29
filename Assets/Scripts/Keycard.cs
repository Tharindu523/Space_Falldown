using UnityEngine;

/// <summary>
/// A keycard with a specific ID.
/// </summary>
public class Keycard : MonoBehaviour
{
    [Header("Key Settings")]
    public int keyID; // Set this to 1, 2, 3, 4, or 5 in the Inspector
    public string keyName = "Access Card";
    public AudioClip pickupSound;

    public void Interact()
    {
        PlayerInteractor player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractor>();
        if (player != null)
        {
            // Add this specific ID to the player's inventory
            player.AddKey(keyID);

            if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // Update Mission UI
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.UpdateObjective("Keycard " + keyID + " acquired. Find the corresponding door.");
            }

            Destroy(gameObject);
        }
    }
}