using UnityEngine;
using System.Collections;
using TMPro; // REQUIRED for using TextMeshPro UI components

/// <summary>
/// Handles all core combat logic: shooting (raycasting), damage application, 
/// fire rate control, ammunition tracking, reloading, and all associated visual/audio feedback.
/// </summary>
public class GunScript : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.5f; // Shots per second (0.5 means 2 shots per second)

    [Header("Ammunition")]
    public int magSize = 30;         // Max bullets the magazine holds
    public float reloadTime = 2.0f;  // Time it takes to reload
    private int currentAmmo;         // Current bullets remaining in the mag
    private bool isReloading = false;

    [Header("Component References")]
    // Drag your Player Camera here (the one this script is attached to)
    public Camera fpsCam;
    private AudioSource audioSource;

    [Header("HUD")]
    // Drag your TextMeshPro UI object here to display ammo count
    public TextMeshProUGUI ammoText;

    [Header("Visual Feedback Prefabs")]
    // The exact point on the gun where the flash should appear.
    public Transform muzzlePoint;
    public GameObject muzzleFlashPrefab;
    public GameObject bulletDecalPrefab;

    [Header("Audio Feedback")]
    public AudioClip fireSound;
    public AudioClip hitSoundEnemy;
    public AudioClip hitSoundEnvironment;
    public AudioClip reloadSound;

    // Time variables for managing the fire rate
    private float nextTimeToFire = 0f;

    void Start()
    {
        // Initialize state and components
        currentAmmo = magSize;

        if (fpsCam == null)
        {
            fpsCam = Camera.main;
            if (fpsCam == null)
            {
                Debug.LogError("GunScript could not find a Camera tagged 'MainCamera'. Set the reference manually.");
                enabled = false;
                return;
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("GunScript requires an AudioSource component on the same GameObject (Camera).");
        }

        UpdateAmmoUI(); // Set initial ammo display
    }

    void Update()
    {
        // Don't do anything if we are currently reloading
        if (isReloading)
            return;

        // 1. Check for Fire Input (Hold Left Mouse Button)
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            if (currentAmmo > 0)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
            else
            {
                // Auto-reload if we try to shoot with no ammo
                StartCoroutine(Reload());
            }
        }

        // 2. Check for Manual Reload Input (Default: R key)
        // Only reload if we have less than a full magazine
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magSize)
        {
            StartCoroutine(Reload());
        }
    }

    /// <summary>
    /// Executes the raycast, damage, and effects logic.
    /// </summary>
    void Shoot()
    {
        // Consume one bullet
        currentAmmo--;
        UpdateAmmoUI();

        // Play the main firing sound
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        // Trigger Muzzle Flash (Visual Feedback)
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            // Instantiate the effect at the muzzle position
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            // Destroy after 0.5 seconds
            StartCoroutine(DestroyMuzzleFlash(flash, 0.5f));
        }

        // Raycast logic
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            Health targetHealth = hit.transform.GetComponent<Health>();

            // --- DAMAGE & HIT SOUND LOGIC ---
            if (targetHealth != null)
            {
                // HIT ENEMY: Apply Damage and Play Enemy Hit Sound
                targetHealth.TakeDamage(damage);
                if (audioSource != null && hitSoundEnemy != null)
                {
                    audioSource.PlayOneShot(hitSoundEnemy);
                }
            }
            else
            {
                // HIT ENVIRONMENT: Play Environment Hit Sound
                if (audioSource != null && hitSoundEnvironment != null)
                {
                    audioSource.PlayOneShot(hitSoundEnvironment);
                }
            }

            // --- BULLET DECAL LOGIC ---
            if (bulletDecalPrefab != null)
            {
                // Align decal rotation with the surface normal
                Quaternion rotation = Quaternion.LookRotation(hit.normal);
                // Add random rotation on Z to prevent uniform bullet holes
                rotation *= Quaternion.Euler(0, 0, Random.Range(0, 360));

                // Instantiate decal slightly offset to prevent Z-fighting
                GameObject decal = Instantiate(bulletDecalPrefab, hit.point + hit.normal * 0.01f, rotation);

                // Attach to the object hit
                decal.transform.SetParent(hit.transform);

                // Destroy after 10 seconds
                Destroy(decal, 10f);
            }
        }
    }

    /// <summary>
    /// Coroutine to handle the reload delay. Prevents firing until complete.
    /// </summary>
    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;
        Debug.Log("Reloading...");

        // Play reload sound
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        // Wait for the specified reload time
        yield return new WaitForSeconds(reloadTime);

        // Refill the magazine
        currentAmmo = magSize;
        isReloading = false;
        UpdateAmmoUI();
        Debug.Log("Reload Complete.");
    }

    /// <summary>
    /// Updates the HUD display for the current ammo count.
    /// </summary>
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + magSize;
        }
    }

    /// <summary>
    /// Destroys a temporary visual effect.
    /// </summary>
    IEnumerator DestroyMuzzleFlash(GameObject flashObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(flashObject);
    }
}