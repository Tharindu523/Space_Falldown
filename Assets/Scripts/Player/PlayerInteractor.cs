using UnityEngine;
using TMPro;
using System.Collections.Generic; // Required for Lists

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3f;
    public Camera playerCamera;

    [Header("Hand References")]
    public GameObject emptyHands;
    public GameObject gunArmedHands;

    [Header("UI Interaction Prompt")]
    public TextMeshProUGUI interactionText;

    [Header("Inventory Logic")]
    public bool hasGun = false;
    public bool hasBomb = false;

    // NEW: List of all keys the player has picked up
    private List<int> heldKeyIDs = new List<int>();

    void Start()
    {
        if (emptyHands != null) emptyHands.SetActive(true);
        if (gunArmedHands != null) gunArmedHands.SetActive(false);
    }

    public void AddKey(int id)
    {
        if (!heldKeyIDs.Contains(id))
        {
            heldKeyIDs.Add(id);
        }
    }

    public bool HasKey(int id)
    {
        return heldKeyIDs.Contains(id);
    }

    public void PickUpWeapon()
    {
        hasGun = true;
        if (emptyHands != null) emptyHands.SetActive(false);
        if (gunArmedHands != null) gunArmedHands.SetActive(true);
        MissionManager.Instance.SetAmmoHUD(true);
    }

    void Update()
    {
        CheckForInteractable();
        if (Input.GetKeyDown(KeyCode.E)) TryInteract();
    }

    void CheckForInteractable()
    {
        RaycastHit hit;
        string prompt = "";

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            if (hit.transform.GetComponent<WeaponPickup>()) prompt = "[E] Take Pulse Rifle";
            else if (hit.transform.GetComponent<BombItem>()) prompt = "[E] Take Fusion Charge";
            else if (hit.transform.GetComponent<Keycard>())
            {
                Keycard k = hit.transform.GetComponent<Keycard>();
                prompt = "[E] Pick up Keycard " + k.keyID;
            }
            else if (hit.transform.GetComponent<DoorScript>())
            {
                DoorScript d = hit.transform.GetComponent<DoorScript>();
                prompt = "[E] Use Console";
            }
            else if (hit.transform.GetComponent<DestructibleObstacle>())
            {
                prompt = hasBomb ? "[E] Plant Explosive" : "Requires Explosive";
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
            WeaponPickup weapon = hit.transform.GetComponent<WeaponPickup>();
            if (weapon != null) { weapon.Interact(); return; }

            BombItem bomb = hit.transform.GetComponent<BombItem>();
            if (bomb != null) { hasBomb = true; bomb.Interact(); MissionManager.Instance.SetBombIcon(true); return; }

            DestructibleObstacle obstacle = hit.transform.GetComponent<DestructibleObstacle>();
            if (obstacle != null && hasBomb) { obstacle.PlantBomb(); hasBomb = false; MissionManager.Instance.SetBombIcon(false); return; }

            Keycard key = hit.transform.GetComponent<Keycard>();
            if (key != null) { key.Interact(); return; }

            DoorScript door = hit.transform.GetComponent<DoorScript>();
            if (door != null) { door.InteractAttempt(this); return; } // Pass 'this' player to check keys
        }
    }
}