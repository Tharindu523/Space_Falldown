using UnityEngine;

/// <summary>
/// Allows the player to toggle a flashlight on and off with the 'F' key.
/// Attach this to a Spot Light that is a child of the Player Camera.
/// </summary>
public class Flashlight : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.F;
    public bool isOn = false;

    [Header("Audio")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    private Light flashlight;

    void Start()
    {
        flashlight = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();

        // Set initial state
        if (flashlight != null)
        {
            flashlight.enabled = isOn;
        }

        if (audioSource == null && clickSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;

        if (flashlight != null)
        {
            flashlight.enabled = isOn;
        }

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        Debug.Log("Flashlight is now " + (isOn ? "ON" : "OFF"));
    }
}