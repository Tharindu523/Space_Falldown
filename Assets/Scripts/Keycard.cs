using UnityEngine;

/// <summary>
/// Attached to the Keycard object. Now triggered by the PlayerInteractor script.
/// </summary>
public class Keycard : MonoBehaviour
{
    [Header("Keycard ID")]
    public string keycardID = "AirlockKey";

    [Header("Audio")]
    public AudioClip pickupSound;
    private AudioSource audioSource;

    private bool isCollected = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    // This is the method the PlayerInteractor will call
    public void Interact()
    {
        if (!isCollected)
        {
            CollectKeycard();
        }
    }

    void CollectKeycard()
    {
        isCollected = true;

        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // Update the static flag so doors know we have a key
        DoorScript.HasKeycard = true;

        // Visual feedback: Hide immediately
        GetComponent<Collider>().enabled = false;
        if (GetComponent<MeshRenderer>() != null) GetComponent<MeshRenderer>().enabled = false;

        Debug.Log("Keycard collected via interaction: " + keycardID);

        // Destroy after sound plays
        Destroy(gameObject, pickupSound != null ? pickupSound.length : 0.1f);
    }
}