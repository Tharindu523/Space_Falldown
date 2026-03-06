using UnityEngine;

/// <summary>
/// Attached to a door object. Handles locking, unlocking, and animation.
/// </summary>
public class DoorScript : MonoBehaviour
{
    [Header("Door Settings")]
    // The keycard ID needed to open this door (must match Keycard.cs keycardID)
    public string requiredKeycardID = "AirlockKey";
    public bool isLocked = true;

    [Header("Component References")]
    // Drag the visual door model (the part that moves) here.
    public Animator doorAnimator;

    // Public property to track if the keycard has been collected
    public static bool HasKeycard = false;

    void Start()
    {
        if (doorAnimator == null)
        {
            Debug.LogError("DoorScript requires an Animator reference on the door model.");
        }
    }

    /// <summary>
    /// Attempts to open the door when the player is near and presses 'E'.
    /// </summary>
    public void InteractAttempt()
    {
        if (!isLocked)
        {
            // Door is already unlocked, so toggle open/close
            ToggleDoor();
        }
        else if (isLocked && HasKeycard)
        {
            // Door is locked, but the player has the key!
            UnlockAndOpen();
        }
        else
        {
            // Door is locked and player lacks the key
            Debug.Log("Door is locked. Requires " + requiredKeycardID + " Keycard.");
            // TODO: Display an on-screen message to the player here
        }
    }

    void ToggleDoor()
    {
        // Check current state and trigger the animation
        bool isOpen = doorAnimator.GetBool("IsOpen");
        doorAnimator.SetBool("IsOpen", !isOpen);
    }

    void UnlockAndOpen()
    {
        isLocked = false;
        ToggleDoor(); // Open the door
        Debug.Log("Door Unlocked and Opened!");
    }

    // NOTE: For the Animator, create a boolean parameter named "IsOpen" 
    // and use it to transition between the Closed and Open states.
}