using UnityEngine;

/// <summary>
/// A door that only opens if the player has a specific Key ID.
/// </summary>
public class DoorScript : MonoBehaviour
{
    [Header("Security Settings")]
    public int requiredKeyID; // The ID of the key needed for this door
    public bool isLocked = true;

    [Header("Animations")]
    private Animator _doorAnim;
    private bool isOpen = false;

    void Start()
    {
        _doorAnim = GetComponentInParent<Animator>();
    }

    public void InteractAttempt(PlayerInteractor player)
    {
        // Check if the player has the matching ID
        if (player.HasKey(requiredKeyID))
        {
            OpenDoor();
        }
        else
        {
            // Show the "Find the key" message via MissionManager
            MissionManager.Instance.ShowLockedMessage("Access Denied. Find Keycard " + requiredKeyID);
            Debug.Log("Door locked. Requires Key ID: " + requiredKeyID);
        }
    }

    void OpenDoor()
    {
        isOpen = !isOpen;

        // Trigger the bool parameter on the parent's Animator
        if (_doorAnim != null)
        {
            _doorAnim.SetBool("IsOpen", isOpen);
        }

        isLocked = false;

        MissionManager.Instance.UpdateObjective("Door Unlocked. Proceed with caution.");
    }
}