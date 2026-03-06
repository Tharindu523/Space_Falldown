using UnityEngine;
using TMPro; // For the interaction hint

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3f;
    public Camera playerCamera;

    [Header("UI Hint")]
    public TextMeshProUGUI interactionText; // Drag a TMP text element here (e.g., "Press E to Interact")

    void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    // Checks what is in front of the player to show/hide the "Press E" hint
    void CheckForInteractable()
    {
        RaycastHit hit;
        bool hitSomething = false;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            if (hit.transform.GetComponent<DoorScript>() || hit.transform.GetComponent<Keycard>())
            {
                hitSomething = true;
            }
        }

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(hitSomething);
        }
    }

    void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            // 1. Check for Door
            DoorScript door = hit.transform.GetComponent<DoorScript>();
            if (door != null)
            {
                door.InteractAttempt();
                return;
            }

            // 2. Check for Keycard
            Keycard key = hit.transform.GetComponent<Keycard>();
            if (key != null)
            {
                key.Interact();
                return;
            }
        }
    }
}