using UnityEngine;

/// <summary>
/// Attached to the gun model on the ground.
/// </summary>
public class WeaponPickup : MonoBehaviour
{
    public string weaponName = "Pulse Rifle";
    public AudioClip pickupSound;

    public void Interact()
    {
        // We call the pickup logic on the player
        PlayerInteractor player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractor>();

        if (player != null)
        {
            player.PickUpWeapon();

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}