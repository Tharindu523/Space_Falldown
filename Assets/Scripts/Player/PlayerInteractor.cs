using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3f;
    public Camera playerCamera;

    [Header("Hand References")]
    [Tooltip("Drag the GameObject representing empty hands/unarmed state here.")]
    public GameObject emptyHands;
    [Tooltip("Drag the GameObject representing the armed state (gun in hand) here.")]
    public GameObject gunArmedHands;

    [Header("UI References")]
    public TextMeshProUGUI interactionText;
    [Tooltip("Drag the UI group or text object for the Ammo HUD here.")]
    public GameObject ammoUI;

    [Header("Inventory State")]
    public bool hasGun = false;
    public bool hasBomb = false;

    void Start()
    {
        // Ensure we start in the correct state: empty hands active, weapon and ammo UI hidden.
        if (!hasGun)
        {
            if (emptyHands != null) emptyHands.SetActive(true);
            if (gunArmedHands != null) gunArmedHands.SetActive(false);
            if (ammoUI != null) ammoUI.SetActive(false);
        }
    }

    void Update()
    {
        CheckForInteractable();
        if (Input.GetKeyDown(KeyCode.E)) TryInteract();
    }

    /// <summary>
    /// Handles the logical switch when a weapon is picked up. 
    /// Disables unarmed hands, enables armed hands, and shows the Ammo HUD.
    /// </summary>
    public void PickUpWeapon()
    {
        hasGun = true;

        // SWITCH HANDS: Disable the "empty" hands and enable the weapon model hands
        if (emptyHands != null) emptyHands.SetActive(false);
        if (gunArmedHands != null) gunArmedHands.SetActive(true);

        // UI FEEDBACK: Show the ammo count HUD now that the player has a gun
        if (ammoUI != null) ammoUI.SetActive(true);

        // Update Objective via MissionManager to reflect progress
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.UpdateObjective("Find a way to clear the path.");
        }

        Debug.Log("Weapon Armed: Hands switched and Ammo UI enabled.");
    }

    void CheckForInteractable()
    {
        RaycastHit hit;
        string prompt = "";

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            // Check for various interactable components and update the on-screen prompt
            if (hit.transform.GetComponent<WeaponPickup>()) prompt = "[E] Pick up Pulse Rifle";
            else if (hit.transform.GetComponent<BombItem>()) prompt = "[E] Pick up Fusion Charge";
            else if (hit.transform.GetComponent<DoorScript>()) prompt = "[E] Open Door";
            else if (hit.transform.GetComponent<Keycard>()) prompt = "[E] Pick up Keycard";
            else if (hit.transform.GetComponent<DestructibleObstacle>())
            {
                prompt = hasBomb ? "[E] Plant Fusion Charge" : "Requires Explosive";
            }
        }

        if (interactionText != null)
        {
            interactionText.text = prompt;
            interactionText.gameObject.SetActive(prompt != "");
        }
    }

    void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            // 1. Check for Weapon Pickup
            WeaponPickup weapon = hit.transform.GetComponent<WeaponPickup>();
            if (weapon != null) { weapon.Interact(); return; }

            // 2. Check for Bomb Pickup
            BombItem bomb = hit.transform.GetComponent<BombItem>();
            if (bomb != null) { bomb.Interact(); MissionManager.Instance.SetBombIcon(true); return; }

            // 3. Check for Obstacle Interaction
            DestructibleObstacle obstacle = hit.transform.GetComponent<DestructibleObstacle>();
            if (obstacle != null && hasBomb) { obstacle.PlantBomb(); hasBomb = false; return; }

            // 4. Check for Keycard or Door Interaction
            Keycard key = hit.transform.GetComponent<Keycard>();
            if (key != null) { key.Interact(); return; }

            DoorScript door = hit.transform.GetComponent<DoorScript>();
            if (door != null) { door.InteractAttempt(); return; }
        }
    }
}