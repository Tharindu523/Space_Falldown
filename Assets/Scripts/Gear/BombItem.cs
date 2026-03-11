using UnityEngine;

/// <summary>
/// Attached to the Bomb/Explosive item in the world.
/// </summary>
public class BombItem : MonoBehaviour
{
    public string bombName = "Fusion Charge";
    public AudioClip pickupSound;

    public void Interact()
    {
        // Tell the player they now have a bomb
        PlayerInteractor player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractor>();
        if (player != null)
        {
            player.hasBomb = true;
            Debug.Log("Picked up: " + bombName);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Hide/Destroy the pickup
            Destroy(gameObject);
        }
    }
}